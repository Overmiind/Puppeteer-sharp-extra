(opts) => {
    class HcaptchaContentScript {
        constructor(opts)
        {
            /** Log using debug flag if available */
            this.log = (message, data) => {
                if (opts && opts.debug) {
                    console.log('[hcaptcha]', message, data);
                }
            };

            this._isVisible = (elem) => !!(elem.offsetWidth || elem.offsetHeight ||
                (typeof elem.getClientRects === 'function' && elem.getClientRects().length));

            // Workaround for https://github.com/esbuild-kit/tsx/issues/113
            if (typeof globalThis.__name === 'undefined') {
                globalThis.__defProp = Object.defineProperty;
                globalThis.__name = (target, value) =>
                    globalThis.__defProp(target, 'name', {
                        value,
                        configurable: true
                    });
            }

            this.opts = opts || {};
            this.data = (opts && opts.data) || {solutions: []};

            this.baseUrls = [
                'assets.hcaptcha.com/captcha/v1/',
                'newassets.hcaptcha.com/captcha/v1/'
            ];

            this.log('Initialized', {
                url: document && document.location ? document.location.href : '',
                opts: this.opts
            });
        }

        /** Check if an element is in the current viewport */
        _isInViewport(elem)
        {
            const rect = elem.getBoundingClientRect();
            return (rect.top >= 0 &&
                rect.left >= 0 &&
                rect.bottom <=
                (window.innerHeight ||
                    (document.documentElement.clientHeight &&
                        rect.right <=
                        (window.innerWidth || document.documentElement.clientWidth))));
        }

        async _waitUntilDocumentReady()
        {
            return new Promise(function (resolve) {
                if (!document || !window) {
                    return resolve(null);
                }
                const ready = /^loaded|^i|^c/.test(document.readyState);
                if (ready) {
                    return resolve(null);
                }

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

        _paintCaptchaBusy($iframe)
        {
            try {
                if (this.opts && this.opts.VisualFeedback) {
                    $iframe.style.filter = 'opacity(60%) hue-rotate(400deg)'; // violet
                }
            } catch {
            }
            return $iframe;
        }

        /** NEW: paint solved (green) */
        _paintCaptchaSolved($iframe)
        {
            try {
                if (this.opts && this.opts.VisualFeedback) {
                    $iframe.style.filter = 'opacity(60%) hue-rotate(230deg)'; // green
                }
            } catch {
            }
            return $iframe;
        }

        _findRegularCheckboxes()
        {
            const selector = this.baseUrls
                .map(
                    url =>
                        `iframe[src*='${url}'][data-hcaptcha-widget-id]:not([src*='invisible'])`
                )
                .join(',');
            return Array.from(document.querySelectorAll(selector));
        }

        _findInvisibleChallenges()
        {
            const selector = this.baseUrls
                .map(
                    url =>
                        `div[style*='visible'] iframe[src*='${url}'][src*='hcaptcha.html']`
                )
                .join(',');
            return Array.from(document.querySelectorAll(selector));
        }

        _extractInfoFromIframes(iframes, captchaType)
        {
            return iframes
                .map(el => {
                    const url = el.src.replace('.html#', '.html?');

                    try {
                        const {searchParams} = new URL(url);

                        return {
                            vendor: 'HCaptcha',
                            captchaType,
                            url: document.location.href,
                            id: searchParams.get('id'),
                            sitekey: (searchParams.get('sitekey') || '').trim(),
                            isInViewport: this._isInViewport(el),
                            hasActiveChallengePopup: false,
                            display: {
                                size: searchParams.get('size') || 'normal'
                            }
                        };
                    } catch (e) {
                        this.log('URL parse error', url);
                        return null;
                    }
                })
                .filter(info => !!info && !!info.sitekey);
        }

        /** Detects both checkbox and invisible hcaptcha */
        async findRecaptchas()
        {
            const result = {captchas: [], error: null};

            try {
                await this._waitUntilDocumentReady();

                const checkboxIframes = this._findRegularCheckboxes();
                const invisibleIframes = this._findInvisibleChallenges();

                this.log('findRecaptchas', {
                    checkbox: checkboxIframes.length,
                    invisible: invisibleIframes.length
                });

                if (!checkboxIframes.length && !invisibleIframes.length) {
                    return result;
                }

                const checkboxInfos = this._extractInfoFromIframes(
                    checkboxIframes,
                    'checkbox'
                );
                const invisibleInfos = this._extractInfoFromIframes(
                    invisibleIframes,
                    'invisible'
                );

                result.captchas = [...checkboxInfos, ...invisibleInfos];

                [...checkboxIframes, ...invisibleIframes].forEach(el =>
                    this._paintCaptchaBusy(el)
                );
            } catch (err) {
                result.error = String(err);
                this.log('findRecaptchas ERROR', result.error);
            }

            return result;
        }

        /** Solve hcaptcha via postMessage protocol */
        async enterRecaptchaSolutions(solutions)
        {
            const result = {
                solved: [],
                error: null
            };

            try {
                await this._waitUntilDocumentReady();

                const effectiveSolutions =
                    Array.isArray(solutions) && solutions.length
                        ? solutions
                        : (this.data && this.data.solutions) || [];

                if (!effectiveSolutions.length) {
                    result.error = 'No solutions provided';
                    return result;
                }

                result.solved = effectiveSolutions.map(solution => {
                    try {
                        if (
                            !solution ||
                            !solution.id ||
                            !solution.text ||
                            solution.vendor !== 'hcaptcha' ||
                            solution.hasSolution !== true
                        ) {
                            return {
                                vendor: 'hcaptcha',
                                id: solution?.id,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: 'Invalid hcaptcha solution'
                            };
                        }

                        // Try to find a visible iframe to mark as solved
                        const solvedIframe =
                            document.querySelector(
                                `iframe[data-hcaptcha-widget-id="${solution.id}"]`
                            ) ||
                            document.querySelector(
                                `iframe[src*="${solution.id}"]`
                            ) ||
                            null;

                        // Send the solution to hcaptcha
                        try {
                            window.postMessage(
                                JSON.stringify({
                                    id: solution.id,
                                    label: 'challenge-closed',
                                    source: 'hcaptcha',
                                    contents: {
                                        event: 'challenge-passed',
                                        expiration: 120,
                                        response: solution.text
                                    }
                                }),
                                '*'
                            );
                        } catch (pmErr) {
                            return {
                                vendor: 'hcaptcha',
                                id: solution.id,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: String(pmErr)
                            };
                        }

                        // Visual success
                        if (solvedIframe) {
                            this._paintCaptchaSolved(solvedIframe);
                        }

                        return {
                            vendor: 'hcaptcha',
                            id: solution.id,
                            isSolved: true,
                            solvedAt: new Date().toISOString()
                        };
                    } catch (err) {
                        return {
                            vendor: 'hcaptcha',
                            id: solution?.id,
                            isSolved: false,
                            solvedAt: new Date().toISOString(),
                            error: String(err)
                        };
                    }
                });
            } catch (err) {
                result.error = String(err);
            }

            return result;
        }
    }

    window.hcaptchaScript = new HcaptchaContentScript(opts);
}
