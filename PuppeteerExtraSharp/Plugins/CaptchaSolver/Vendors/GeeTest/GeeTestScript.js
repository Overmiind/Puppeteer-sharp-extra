(opts) => {
    class GeeTestContentScript {
        constructor(opts)
        {
            this.opts = opts || {};
            this.log = (message, data) => {
                if (this.opts.debug) {
                    console.log('[geetest]', message, data);
                }
            };

            // Workaround for https://github.com/esbuild-kit/tsx/issues/113
            if (typeof globalThis.__name === 'undefined') {
                globalThis.__defProp = Object.defineProperty;
                globalThis.__name = (target, value) =>
                    globalThis.__defProp(target, 'name', {
                        value,
                        configurable: true
                    });
            }

            this.log('Initialized GeeTestContentScript', {
                url: document.location && document.location.href
            });
        }

        // Equivalent à jQuery :visible
        _isVisible(elem)
        {
            return !!(
                elem &&
                (elem.offsetWidth ||
                    elem.offsetHeight ||
                    (typeof elem.getClientRects === 'function' &&
                        elem.getClientRects().length))
            );
        }

        _isInViewport(elem)
        {
            if (!elem) return false;
            const rect = elem.getBoundingClientRect();
            const vpHeight =
                window.innerHeight ||
                (document.documentElement && document.documentElement.clientHeight) ||
                0;
            const vpWidth =
                window.innerWidth ||
                (document.documentElement && document.documentElement.clientWidth) ||
                0;

            return (
                rect.top < vpHeight &&
                rect.left < vpWidth &&
                rect.bottom > 0 &&
                rect.right > 0
            );
        }

        _paintCaptchaBusy(elem)
        {
            try {
                if (this.opts.VisualFeedback || this.opts.visualFeedback) {
                    elem.style.filter = 'opacity(60%) hue-rotate(400deg)'; // violet
                }
            } catch (e) {
                // noop
            }
            return elem;
        }

        _paintCaptchaSolved(elem)
        {
            try {
                if (this.opts.VisualFeedback || this.opts.visualFeedback) {
                    elem.style.filter = 'opacity(60%) hue-rotate(230deg)'; // green
                }
            } catch (e) {
                // noop
            }
            return elem;
        }

        async _waitUntilDocumentReady()
        {
            return new Promise((resolve) => {
                if (!document || !window) return resolve(null);
                const loaded = /^loaded|^i|^c/.test(document.readyState);
                if (loaded) return resolve(null);

                function onReady()
                {
                    resolve(null);
                    document.removeEventListener('DOMContentLoaded', onReady);
                    window.removeEventListener('load', onReady);
                }

                document.addEventListener('DOMContentLoaded', onReady);
                window.addEventListener('load', onReady);
            });
        }

        async _refreshChallengeAsync()
        {
            const url = new URL(window.__geetest_url);
            url.searchParams.set("t", Date.now());

            const response = await fetch(url.toString());
            if (!response.ok) return null;
            const data = await response.json();

            if (data && data.challenge) {
                window.__geetest_challenge = data.challenge;
            }
        }

        /**
         * On essaie de trouver les containers GeeTest:
         *  - div.geetest_holder
         *  - div[class*="geetest"]
         *  - inputs hidden geetest_challenge / geetest_validate
         */
        _findGeeTestContainers()
        {
            const holders = Array.from(
                document.querySelectorAll('.geetest_holder')
            );

            if (holders.length) {
                return holders;
            }

            const challengeInputs = Array.from(
                document.querySelectorAll("input[name='geetest_challenge']")
            );

            if (!challengeInputs.length) {
                return [];
            }

            const containers = challengeInputs
                .map((inp) =>
                    inp.closest('.geetest_holder, form, div') || document.body
                );

            const unique = [];
            const seen = new Set();
            for (const el of containers) {
                if (!el) continue;
                if (seen.has(el)) continue;
                seen.add(el);
                unique.push(el);
            }

            return unique;
        }

        _extractInfoFromContainer(container, index)
        {
            if (!container) return null;

            let id = container.getAttribute('data-geetest-id') || container.getAttribute('id');
            if (!id) {
                id = 'geetest-' + index;
                container.setAttribute('data-geetest-id', id);
            }

            const challengeInput =
                container.querySelector("input[name='geetest_challenge']") ||
                document.querySelector("input[name='geetest_challenge']");

            const validateInput =
                container.querySelector("input[name='geetest_validate']") ||
                document.querySelector("input[name='geetest_validate']");

            const seccodeInput =
                container.querySelector("input[name='geetest_seccode']") ||
                document.querySelector("input[name='geetest_seccode']");

            let gt = challengeInput?.getAttribute('data-gt') || container.getAttribute('data-gt') || window.__geetest_gt || null;
            let challenge = challengeInput?.value || window.__geetest_challenge || null;
            let captchaId = window.__geetest_captcha_id || null;

            let version = null;
            if (captchaId) {
                version = 'v4';
            } else if (challengeInput || validateInput || seccodeInput || challenge) {
                version = 'v3';
            }

            const sitekey = captchaId || gt || null;

            const info = {
                vendor: 'geetest',
                captchaType: 'checkbox',
                url: document.location && document.location.href,
                id: id,
                sitekey: sitekey,
                gt: gt,
                challenge: challenge,
                captchaId: captchaId,
                version: version,
                isInViewport: this._isInViewport(container),
                hasActiveChallengePopup: this._isInViewport(container),
                display: {
                    hasChallengeInput: !!challengeInput,
                    hasValidateInput: !!validateInput,
                    hasSeccodeInput: !!seccodeInput
                }
            };

            return info;
        }

        async findCaptchas()
        {
            const result = {
                captchas: [],
                error: null
            };

            try {
                await this._waitUntilDocumentReady();

                const containers = this._findGeeTestContainers();
                this.log('findCaptchas (geetest)', {count: containers.length});

                if (!containers.length) {
                    return result;
                }

                await this._refreshChallengeAsync();
                result.captchas = containers
                    .filter((c) => this._isVisible(c))
                    .map((c, index) => {
                        this._paintCaptchaBusy(c);
                        return this._extractInfoFromContainer(c, index);
                    })
                    .filter((info) => !!info);

                this.log('findCaptchas (geetest) - result', {
                    captchaNum: result.captchas.length,
                    result
                });
            } catch (e) {
                result.error = String(e);
                this.log('findCaptchas (geetest) - ERROR', String(e));
            }

            return result;
        }

        /**
         * solutions: tableau d'objets du type:
         * {
         *   vendor: 'geetest',
         *   id: 'geetest-0' (ou autre),
         *   text: '{"challenge":"...","validate":"...","seccode":"..."}'
         *   // ou:
         *   challenge: '...',
         *   validate: '...',
         *   seccode: '...'
         * }
         */
        async enterCaptchaSolutions(solutions)
        {
            const result = {
                solved: [],
                error: null
            };

            try {
                await this._waitUntilDocumentReady();

                const effectiveSolutions = Array.isArray(solutions) ? solutions : [];
                this.log('enterCaptchaSolutions (geetest)', {
                    solutionNum: effectiveSolutions.length
                });

                if (!effectiveSolutions.length) {
                    result.error = 'No solutions provided';
                    return result;
                }

                result.solved = effectiveSolutions.map((solution) => {
                    try {
                        const payload = typeof solution.payload === 'string' ? JSON.parse(solution.payload) : null;
                        const vendor = solution.vendor;

                        if (vendor !== 'geetest') {
                            return {
                                vendor: 'geetest',
                                id: solution && solution.id ? solution.id : undefined,
                                responseElement: false,
                                responseCallback: false,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: 'Not a geetest solution'
                            };
                        }

                        if (!solution || !solution.id) {
                            return {
                                vendor: 'geetest',
                                id: undefined,
                                responseElement: false,
                                responseCallback: false,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: 'Invalid solution payload (missing id)'
                            };
                        }

                        if (!payload.challenge || !payload.validate || !payload.seccode) {
                            return {
                                vendor: 'geetest',
                                id: solution.id,
                                responseElement: false,
                                responseCallback: false,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: 'Missing challenge/validate/seccode'
                            };
                        }

                        const container =
                            document.querySelector(`[data-geetest-id="${solution.id}"]`) ||
                            document.getElementById(solution.id) ||
                            document.body;

                        const challengeInput =
                            container.querySelector("input[name='geetest_challenge']") ||
                            document.querySelector("input[name='geetest_challenge']");

                        const validateInput =
                            container.querySelector("input[name='geetest_validate']") ||
                            document.querySelector("input[name='geetest_validate']");

                        const seccodeInput =
                            container.querySelector("input[name='geetest_seccode']") ||
                            document.querySelector("input[name='geetest_seccode']");

                        let responseElement = false;

                        const setValueWithEvents = (input, value) => {
                            try {
                                input.value = value;
                                input.dispatchEvent(new Event('input', {bubbles: true}));
                                input.dispatchEvent(new Event('change', {bubbles: true}));
                                return true;
                            } catch (e) {
                                this.log('Error setting input', String(e));
                                return false;
                            }
                        };

                        if (challengeInput) {
                            responseElement = setValueWithEvents(challengeInput, payload.challenge) || responseElement;
                        }
                        if (validateInput) {
                            responseElement = setValueWithEvents(validateInput, payload.validate) || responseElement;
                        }
                        if (seccodeInput) {
                            responseElement = setValueWithEvents(seccodeInput, payload.seccode) || responseElement;
                        }

                        const solved = {
                            vendor: 'geetest',
                            id: solution.id,
                            responseElement: responseElement,
                            responseCallback: false,
                            isSolved: !!responseElement,
                            solvedAt: new Date().toISOString()
                        };

                        if (!responseElement) {
                            solved.error = 'Could not locate GeeTest hidden inputs';
                        }

                        this.log('enterCaptchaSolutions (geetest) - solved', solved);
                        return solved;
                    } catch (e) {
                        return {
                            vendor: 'geetest',
                            id: solution && solution.id ? solution.id : undefined,
                            responseElement: false,
                            responseCallback: false,
                            isSolved: false,
                            solvedAt: new Date().toISOString(),
                            error: String(e)
                        };
                    }
                });
            } catch (e) {
                result.error = String(e);
                this.log('enterCaptchaSolutions (geetest) - ERROR', String(e));
            }

            return result;
        }
    }

    window.geeTestScript = new GeeTestContentScript(opts);
}
