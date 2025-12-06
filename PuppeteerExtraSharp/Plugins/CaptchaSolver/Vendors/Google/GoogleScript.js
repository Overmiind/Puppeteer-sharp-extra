(opts) => {
    class RecaptchaContentScript {
        constructor(opts)
        {
            /** Log using debug binding if available */
            this.log = (message, data) => {
                if (opts.debug) {
                    console.log(message, data);
                }
            };
            // Poor mans _.pluck
            this._pick = (props) => (o) => props.reduce((a, e) => (Object.assign(Object.assign({}, a), {[e]: o[e]})), {});
            // make sure the element is visible - this is equivalent to jquery's is(':visible')
            this._isVisible = (elem) => !!(elem.offsetWidth ||
                elem.offsetHeight ||
                (typeof elem.getClientRects === 'function' &&
                    elem.getClientRects().length));
            // Workaround for https://github.com/esbuild-kit/tsx/issues/113
            if (typeof globalThis.__name === 'undefined') {
                globalThis.__defProp = Object.defineProperty;
                globalThis.__name = (target, value) => globalThis.__defProp(target, 'name', {
                    value,
                    configurable: true
                });
            }
            this.opts = opts;

            this.frameSources = this._generateFrameSources();
            this.log('Intialized', {url: document.location.href, opts: this.opts});
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

        // Recaptcha client is a nested, circular object with object keys that seem generated
        // We flatten that object a couple of levels deep for easy access to certain keys we're interested in.
        _flattenObject(item, levels = 2, ignoreHTML = true)
        {
            const isObject = (x) => x && typeof x === 'object';
            const isHTML = (x) => x && x instanceof HTMLElement;
            let newObj = {};
            for (let i = 0; i < levels; i++) {
                item = Object.keys(newObj).length ? newObj : item;
                Object.keys(item).forEach(key => {
                    if (ignoreHTML && isHTML(item[key]))
                        return;
                    if (isObject(item[key])) {
                        Object.keys(item[key]).forEach(innerKey => {
                            if (ignoreHTML && isHTML(item[key][innerKey]))
                                return;
                            const keyName = isObject(item[key][innerKey])
                                ? `obj_${key}_${innerKey}`
                                : `${innerKey}`;
                            newObj[keyName] = item[key][innerKey];
                        });
                    } else {
                        newObj[key] = item[key];
                    }
                });
            }
            return newObj;
        }

        // Helper function to return an object based on a well known value
        _getKeyByValue(object, value)
        {
            return Object.keys(object).find(key => object[key] === value);
        }

        async _waitUntilDocumentReady()
        {
            return new Promise(function (resolve) {
                if (!document || !window) {
                    return resolve(null);
                }
                const loadedAlready = /^loaded|^i|^c/.test(document.readyState);
                if (loadedAlready) {
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
                if (this.opts.VisualFeedback) {
                    $iframe.style.filter = `opacity(60%) hue-rotate(400deg)`; // violet
                }
            } catch (error) {
                // noop
            }
            return $iframe;
        }

        _paintCaptchaSolved($iframe)
        {
            try {
                if (this.opts.VisualFeedback) {
                    $iframe.style.filter = `opacity(60%) hue-rotate(230deg)`; // green
                }
            } catch (error) {
                // noop
            }
            return $iframe;
        }

        _findVisibleIframeNodes()
        {
            return Array.from(document.querySelectorAll(this.getFrameSelectorForId('anchor', '') // intentionally blank
            ));
        }

        _findVisibleIframeNodeById(id)
        {
            return document.querySelector(this.getFrameSelectorForId('anchor', id));
        }

        _hideChallengeWindowIfPresent(id = '')
        {
            let frame = document.querySelector(this.getFrameSelectorForId('bframe', id));
            this.log(' - _hideChallengeWindowIfPresent', {id, hasFrame: !!frame});
            if (!frame) {
                return;
            }
            while (frame &&
            frame.parentElement &&
            frame.parentElement !== document.body) {
                frame = frame.parentElement;
            }
            if (frame) {
                frame.style.visibility = 'hidden';
            }
        }

        // There's so many different possible deployments URLs that we better generate them
        _generateFrameSources()
        {
            const protos = ['http', 'https'];
            const hosts = [
                'google.com',
                'www.google.com',
                'recaptcha.net',
                'www.recaptcha.net'
            ];
            const origins = protos.flatMap(proto => hosts.map(host => `${proto}://${host}`));
            const paths = {
                anchor: ['/recaptcha/api2/anchor', '/recaptcha/enterprise/anchor'],
                bframe: ['/recaptcha/api2/bframe', '/recaptcha/enterprise/bframe']
            };
            return {
                anchor: origins.flatMap(origin => paths.anchor.map(path => `${origin}${path}`)),
                bframe: origins.flatMap(origin => paths.bframe.map(path => `${origin}${path}`))
            };
        }

        getFrameSelectorForId(type = 'anchor', id = '')
        {
            const namePrefix = type === 'anchor' ? 'a' : 'c';
            return this.frameSources[type]
                .map(src => `iframe[src^='${src}'][name^="${namePrefix}-${id}"]`)
                .join(',');
        }

        getClients()
        {
            // Bail out early if there's no indication of recaptchas
            if (!window || !window.__google_recaptcha_client)
                return;
            if (!window.___grecaptcha_cfg || !window.___grecaptcha_cfg.clients) {
                return;
            }
            if (!Object.keys(window.___grecaptcha_cfg.clients).length)
                return;
            return window.___grecaptcha_cfg.clients;
        }

        getVisibleIframesIds()
        {
            // Find all regular visible recaptcha boxes through their iframes
            const result = this._findVisibleIframeNodes()
                .filter($f => this._isVisible($f))
                .map($f => this._paintCaptchaBusy($f))
                .filter($f => $f && $f.getAttribute('name'))
                .map($f => $f.getAttribute('name') || '') // a-841543e13666
                .map(rawId => rawId.split('-').slice(-1)[0] // a-841543e13666 => 841543e13666
                )
                .filter(id => id);
            this.log('getVisibleIframesIds', result);
            return result;
        }

        // TODO: Obsolete with recent changes
        getInvisibleIframesIds()
        {
            // Find all invisible recaptcha boxes through their iframes (only the ones with an active challenge window)
            const result = this._findVisibleIframeNodes()
                .filter($f => $f && $f.getAttribute('name'))
                .map($f => $f.getAttribute('name') || '') // a-841543e13666
                .map(rawId => rawId.split('-').slice(-1)[0] // a-841543e13666 => 841543e13666
                )
                .filter(id => id)
                .filter(id => document.querySelectorAll(this.getFrameSelectorForId('bframe', id))
                    .length);
            this.log('getInvisibleIframesIds', result);
            return result;
        }

        getIframesIds()
        {
            // Find all recaptcha boxes through their iframes, check for invisible ones as fallback
            const results = [
                ...this.getVisibleIframesIds(),
                ...this.getInvisibleIframesIds()
            ];
            this.log('getIframesIds', results);
            // Deduplicate results by using the unique id as key
            const dedup = Array.from(new Set(results));
            this.log('getIframesIds - dedup', dedup);
            return dedup;
        }

        isEnterpriseCaptcha(id)
        {
            if (!id)
                return false;
            // The only way to determine if a captcha is an enterprise one is by looking at their iframes
            const prefix = 'iframe[src*="/recaptcha/"][src*="/enterprise/"]';
            const nameSelectors = [`[name^="a-${id}"]`, `[name^="c-${id}"]`];
            const fullSelector = nameSelectors.map(name => prefix + name).join(',');
            return document.querySelectorAll(fullSelector).length > 0;
        }

        isInvisible(id)
        {
            if (!id)
                return false;
            const selector = `iframe[src*="/recaptcha/"][src*="/anchor"][name="a-${id}"][src*="&size=invisible"]`;
            return document.querySelectorAll(selector).length > 0;
        }

        /** Whether an active challenge popup is open */
        hasActiveChallengePopup(id)
        {
            if (!id)
                return false;
            const selector = `iframe[src*="/recaptcha/"][src*="/bframe"][name="c-${id}"]`;
            const elem = document.querySelector(selector);
            if (!elem) {
                return false;
            }
            return this._isInViewport(elem); // note: _isVisible doesn't work here as the outer div is hidden, not the iframe itself
        }

        /** Whether an (invisible) captcha has a challenge bframe - otherwise it's a score based captcha */
        hasChallengeFrame(id)
        {
            if (!id)
                return false;
            return (document.querySelectorAll(this.getFrameSelectorForId('bframe', id))
                .length > 0);
        }

        isInViewport(id)
        {
            if (!id)
                return;
            const prefix = 'iframe[src*="recaptcha"]';
            const nameSelectors = [`[name^="a-${id}"]`, `[name^="c-${id}"]`];
            const fullSelector = nameSelectors.map(name => prefix + name).join(',');
            const elem = document.querySelector(fullSelector);
            if (!elem) {
                return false;
            }
            return this._isInViewport(elem);
        }

        getResponseInputById(id)
        {
            if (!id)
                return;
            const $iframe = this._findVisibleIframeNodeById(id);
            if (!$iframe)
                return;
            const $parentForm = $iframe.closest(`form`);
            if ($parentForm) {
                return $parentForm.querySelector(`[name='g-recaptcha-response']`);
            }
            // Not all reCAPTCHAs are in forms
            // https://github.com/berstend/puppeteer-extra/issues/57
            if (document && document.body) {
                return document.body.querySelector(`[name='g-recaptcha-response']`);
            }
        }

        getClientById(id)
        {
            if (!id)
                return;
            const clients = this.getClients();
            // Lookup captcha "client" info using extracted id
            let client = Object.values(clients || {})
                .filter(obj => this._getKeyByValue(obj, id))
                .shift(); // returns first entry in array or undefined
            this.log(' - getClientById:client', {id, hasClient: !!client});
            if (!client)
                return;
            try {
                client = this._flattenObject(client);
                client.widgetId = client.id;
                client.id = id;
                this.log(' - getClientById:client:flatten', {
                    id,
                    hasClient: !!client
                });
            } catch (err) {
                this.log(' - getClientById:client ERROR', err.toString());
            }
            return client;
        }

        extractInfoFromClient(client)
        {
            if (!client)
                return;
            const info = this._pick(['sitekey', 'callback'])(client);
            if (!info.sitekey)
                return;
            info.vendor = 'Google';
            info.id = client.id;
            info.s = client.s; // google site specific
            info.widgetId = client.widgetId;
            info.display = this._pick([
                'size',
                'top',
                'left',
                'width',
                'height',
                'theme'
            ])(client);
            if (client && client.action) {
                info.action = client.action;
            }
            // callbacks can be strings or funtion refs
            if (info.callback && typeof info.callback === 'function') {
                info.callback = info.callback.name || 'anonymous';
            }
            if (document && document.location)
                info.url = document.location.href;
            return info;
        }

        async findRecaptchas()
        {
            const result = {
                captchas: [],
                error: null
            };
            try {
                await this._waitUntilDocumentReady();
                const clients = this.getClients();
                this.log('findRecaptchas', {
                    url: document.location.href,
                    hasClients: !!clients
                });
                if (!clients)
                    return result;
                result.captchas = this.getIframesIds()
                    .map(id => this.getClientById(id))
                    .map(client => this.extractInfoFromClient(client))
                    .map(info => {
                        this.log(' - captchas:info', info);
                        if (!info)
                            return;
                        const $input = this.getResponseInputById(info.id);
                        info.hasResponseElement = !!$input;
                        return info;
                    })
                    .filter(info => !!info && !!info.sitekey)
                    .map(info => {
                        info.sitekey = info.sitekey.trim();
                        info.isEnterprise = this.isEnterpriseCaptcha(info.id);
                        info.isInViewport = this.isInViewport(info.id);
                        info.isInvisible = this.isInvisible(info.id);
                        info.captchaType = 'checkbox';
                        if (info.isInvisible) {
                            info.captchaType = 'invisible';
                            info.hasActiveChallengePopup = this.hasActiveChallengePopup(info.id);
                            info.hasChallengeFrame = this.hasChallengeFrame(info.id);
                            if (!info.hasChallengeFrame) {
                                info.captchaType = 'score';
                            }
                        }
                        return info;
                    });
            } catch (error) {
                result.error = error;
                return result;
            }
            this.log('findRecaptchas - result', {
                captchaNum: result.captchas.length,
                result
            });
            return result;
        }

        async enterRecaptchaSolutions(solutions)
        {
            const result = {
                solved: [],
                error: null
            };

            try {
                await this._waitUntilDocumentReady();

                const effectiveSolutions = Array.isArray(solutions) ? solutions : [];
                this.log('enterCaptchaSolutions (google)', {
                    solutionNum: effectiveSolutions.length
                });

                if (!effectiveSolutions.length) {
                    result.error = 'No solutions provided';
                    return result;
                }

                const clients = this.getClients();
                this.log('enterRecaptchaSolutions', {
                    url: document && document.location ? document.location.href : '',
                    hasClients: !!clients,
                    solutionNum: Array.isArray(solutions) ? solutions.length : 0
                });

                if (!clients) {
                    result.error = 'No recaptchas found';
                    return result;
                }
                if (!Array.isArray(solutions) || solutions.length === 0) {
                    result.error = 'No solutions provided';
                    return result;
                }

                result.solved = solutions.map(solution => {
                    try {
                        const payload = typeof solution.payload === 'string' ? JSON.parse(solution.payload) : null;
                        const vendor = solution.vendor;

                        if (vendor !== 'Google') {
                            return {
                                vendor: 'Google',
                                id: solution && solution.id ? solution.id : undefined,
                                responseElement: false,
                                responseCallback: false,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: 'Not a google solution'
                            };
                        }

                        if (!solution || !solution.id) {
                            return {
                                vendor: 'Google',
                                id: undefined,
                                responseElement: false,
                                responseCallback: false,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: 'Invalid solution payload (missing id)'
                            };
                        }

                        const client = this.getClientById(solution.id);
                        this.log(' - client', !!client);
                        if (!client) {
                            return {
                                vendor: 'Google',
                                id: solution.id,
                                responseElement: false,
                                responseCallback: false,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: `Client not found for id '${solution.id}'`
                            };
                        }

                        const solved = {
                            vendor: 'Google',
                            id: client.id,
                            responseElement: false,
                            responseCallback: false
                        };

                        const $iframe = this._findVisibleIframeNodeById(solved.id);
                        this.log(' - $iframe', !!$iframe);
                        if (!$iframe) {
                            return {
                                ...solved,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: `Iframe not found for id '${solved.id}'`
                            };
                        }

                        // Enter solution in response textarea
                        // 1) Try id-specific textarea first (preferred when multiple widgets exist)
                        let $input =
                            document.getElementById(`g-recaptcha-response-${solved.id}`) ||
                            this.getResponseInputById(solved.id) ||
                            document.querySelector(`#g-recaptcha-response`) ||
                            document.querySelector(`[name='g-recaptcha-response']`);

                        this.log(' - $input', !!$input);
                        if ($input) {
                            try {
                                // Use value, not innerHTML
                                $input.value = payload.gRecaptchaResponse;

                                // Fire input/change events
                                $input.dispatchEvent(new Event('input', {bubbles: true}));
                                $input.dispatchEvent(new Event('change', {bubbles: true}));

                                solved.responseElement = true;
                            } catch (e) {
                                this.log(' - set input ERROR', String(e));
                            }
                        }

                        // Enter solution in optional callback
                        // Avoid eval; prefer function or window[name]
                        const cb = client.callback;
                        this.log(' - callback', !!cb);
                        if (cb) {
                            try {
                                let fn = null;
                                if (typeof cb === 'function') {
                                    fn = cb;
                                } else if (typeof cb === 'string') {
                                    // Try resolve from window by name (works if page assigned it to window)
                                    fn = typeof window[cb] === 'function' ? window[cb] : null;
                                }

                                if (typeof fn === 'function') {
                                    fn.call(window, payload.gRecaptchaResponse);
                                    solved.responseCallback = true;
                                } else {
                                    this.log(' - callback unresolved', {
                                        type: typeof cb,
                                        value: String(cb).slice(0, 200)
                                    });
                                }
                            } catch (error) {
                                this.log(' - callback ERROR', String(error));
                            }
                        }

                        // Finishing up
                        solved.isSolved = !!(solved.responseCallback || solved.responseElement);
                        solved.solvedAt = new Date().toISOString();

                        if (solved.isSolved) {
                            // Hide challenge only after we have set the token/callback
                            if (this.hasActiveChallengePopup(solved.id)) {
                                this._hideChallengeWindowIfPresent(solved.id);
                            }
                            this._paintCaptchaSolved($iframe);
                        }

                        this.log(' - solved', {
                            id: solved.id,
                            responseElement: solved.responseElement,
                            responseCallback: solved.responseCallback,
                            isSolved: solved.isSolved
                        });

                        return solved;
                    } catch (e) {
                        return {
                            vendor: 'Google',
                            id: solution && solution.id ? solution.id : undefined,
                            responseElement: false,
                            responseCallback: false,
                            isSolved: false,
                            solvedAt: new Date().toISOString(),
                            error: String(e)
                        };
                    }
                });
            } catch (error) {
                result.error = String(error);
                this.log('enterCaptchaSolutions (cloudflare) - ERROR', String(e));
            }

            this.log('enterRecaptchaSolutions - finished', {
                solvedCount: result.solved.filter(s => s.isSolved).length,
                total: result.solved.length,
                hasError: !!result.error
            });

            return result;
        }
    }

    window.reScript = new RecaptchaContentScript(opts)
}
