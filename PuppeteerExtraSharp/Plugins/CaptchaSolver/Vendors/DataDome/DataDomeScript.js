(opts) => {
    class DataDomeContentScript {
        constructor(opts) {
            this.opts = opts || {};
            this.log = (message, data) => {
                if (this.opts.debug || this.opts.Debug) {
                    console.log('[DataDome]', message, data);
                }
            };

            if (typeof globalThis.__name === 'undefined') {
                globalThis.__defProp = Object.defineProperty;
                globalThis.__name = (target, value) =>
                    globalThis.__defProp(target, 'name', { value, configurable: true });
            }

            this.log('Initialized DataDomeContentScript', {
                url: document.location && document.location.href
            });
        }

        _isVisible(elem) {
            return !!(
                elem &&
                (elem.offsetWidth ||
                    elem.offsetHeight ||
                    (typeof elem.getClientRects === 'function' &&
                        elem.getClientRects().length))
            );
        }

        _isInViewport(elem) {
            if (!elem) return false;
            const rect = elem.getBoundingClientRect();
            const vpHeight = window.innerHeight ||
                (document.documentElement && document.documentElement.clientHeight) || 0;
            const vpWidth = window.innerWidth ||
                (document.documentElement && document.documentElement.clientWidth) || 0;

            return (
                rect.top < vpHeight &&
                rect.left < vpWidth &&
                rect.bottom > 0 &&
                rect.right > 0
            );
        }

        _paintCaptchaBusy(elem) {
            try {
                if (this.opts.VisualFeedback || this.opts.visualFeedback) {
                    elem.style.filter = 'opacity(60%) hue-rotate(400deg)';
                }
            } catch (e) { }
            return elem;
        }

        _paintCaptchaSolved(elem) {
            try {
                if (this.opts.VisualFeedback || this.opts.visualFeedback) {
                    elem.style.filter = 'opacity(60%) hue-rotate(230deg)';
                }
            } catch (e) { }
            return elem;
        }

        async _waitUntilDocumentReady() {
            return new Promise((resolve) => {
                if (!document || !window) return resolve(null);
                const loaded = /^loaded|^i|^c/.test(document.readyState);
                if (loaded) return resolve(null);

                function onReady() {
                    resolve(null);
                    document.removeEventListener('DOMContentLoaded', onReady);
                    window.removeEventListener('load', onReady);
                }

                document.addEventListener('DOMContentLoaded', onReady);
                window.addEventListener('load', onReady);
            });
        }

        /**
         * Find DataDome captcha containers:
         *  - iframe[src*="geo.captcha-delivery.com"]
         *  - iframe[src*="datadome"]
         *  - #datadome-captcha
         *  - Interstitial pages with DataDome challenge
         */
        _findDataDomeContainers() {
            // Look for DataDome iframes
            const iframes = Array.from(document.querySelectorAll(
                'iframe[src*="geo.captcha-delivery.com"], ' +
                'iframe[src*="datadome"], ' +
                'iframe[src*="captcha-delivery"]'
            ));

            if (iframes.length) {
                return iframes;
            }

            // Look for specific DataDome containers
            const containers = Array.from(document.querySelectorAll(
                '#datadome-captcha, .datadome-captcha, [data-datadome]'
            ));

            if (containers.length) {
                return containers;
            }

            // Check for interstitial/full page challenge
            // DataDome may inject the challenge into the page body
            const interstitialIndicators = document.querySelectorAll(
                'script[src*="datadome"], ' +
                'script[src*="captcha-delivery"], ' +
                'link[href*="datadome"]'
            );

            if (interstitialIndicators.length) {
                // The whole page is likely a DataDome challenge
                const body = document.body;
                if (body && body.innerHTML.includes('datadome')) {
                    return [body];
                }
            }

            return [];
        }

        _detectCaptchaType(container) {
            // Detect which type of DataDome captcha this is
            // Must return values matching CaptchaType enum: 'invisible', 'checkbox', 'score'
            const src = container.src || '';
            const html = container.innerHTML || '';
            const captchaUrl = window.__datadome_captcha_url || '';
            const t = window.__datadome_t || '';

            // Device check is invisible/automatic
            if (src.includes('device') || html.includes('device-check')) {
                return 'invisible';
            }

            // Check the t parameter first (most reliable)
            // 'fe' = frontend challenge (slider), 'bv' = bot verification
            if (t === 'fe' || t === 'bv') {
                return 'checkbox'; // Interactive challenge = checkbox type
            }
            if (t === 'interstitial') {
                return 'checkbox'; // Full page challenge = checkbox type
            }

            if (src.includes('slider') || html.includes('slider') || captchaUrl.includes('slider')) {
                return 'checkbox';
            }
            if (src.includes('puzzle') || html.includes('puzzle') || captchaUrl.includes('interstitial')) {
                return 'checkbox';
            }

            // Default to checkbox as most DataDome challenges are interactive
            return 'checkbox';
        }

        _extractInfoFromContainer(container, index) {
            if (!container) return null;

            let id = container.getAttribute && (
                container.getAttribute('data-datadome-id') ||
                container.getAttribute('id')
            );
            if (!id) {
                id = 'datadome-' + index;
                try {
                    if (container.setAttribute) {
                        container.setAttribute('data-datadome-id', id);
                    }
                } catch (e) { }
            }

            // Extract captcha URL from iframe src or window variable
            let captchaUrl = (container.src) || window.__datadome_captcha_url || null;

            // Extract parameters from URL or window variables
            let cid = window.__datadome_cid || null;
            let hash = window.__datadome_hash || null;
            let t = window.__datadome_t || null;
            let s = window.__datadome_s || null;
            let referer = window.__datadome_referer || document.referrer || null;

            // Try to extract from URL if available
            if (captchaUrl) {
                try {
                    const url = new URL(captchaUrl);
                    cid = cid || url.searchParams.get('cid');
                    hash = hash || url.searchParams.get('hash') || url.searchParams.get('hsh');
                    t = t || url.searchParams.get('t');
                    s = s || url.searchParams.get('s');
                    referer = referer || url.searchParams.get('referer');
                } catch (e) {
                    this.log('Error parsing captcha URL', e);
                }
            }

            const captchaType = this._detectCaptchaType(container);

            const info = {
                vendor: 'DataDome',
                captchaType: captchaType,
                url: document.location && document.location.href,
                id: id,
                sitekey: cid, // Use cid as sitekey equivalent
                dataDomeCaptchaUrl: captchaUrl,
                dataDomeCid: cid,
                dataDomeHash: hash,
                dataDomeT: t,
                dataDomeS: s,
                dataDomeReferer: referer,
                dataDomeUserAgent: navigator.userAgent,
                isInViewport: this._isInViewport(container),
                hasActiveChallengePopup: true,
                display: {
                    type: captchaType,
                    hasCid: !!cid,
                    hasHash: !!hash
                }
            };

            return info;
        }

        async findCaptchas() {
            const result = {
                captchas: [],
                error: null
            };

            try {
                await this._waitUntilDocumentReady();

                const containers = this._findDataDomeContainers();
                this.log('findCaptchas (datadome)', { count: containers.length });

                if (!containers.length) {
                    return result;
                }

                result.captchas = containers
                    .filter((c) => this._isVisible(c))
                    .map((c, index) => {
                        this._paintCaptchaBusy(c);
                        return this._extractInfoFromContainer(c, index);
                    })
                    .filter((info) => !!info);

                this.log('findCaptchas (datadome) - result', {
                    captchaNum: result.captchas.length,
                    result
                });
            } catch (e) {
                result.error = String(e);
                this.log('findCaptchas (datadome) - ERROR', String(e));
            }

            return result;
        }

        /**
         * Enter solutions - DataDome typically requires setting a cookie
         * or injecting a token into the page
         */
        async enterCaptchaSolutions(solutions) {
            const result = {
                solved: [],
                error: null,
                needsReload: false
            };

            try {
                await this._waitUntilDocumentReady();

                const effectiveSolutions = Array.isArray(solutions) ? solutions : [];
                this.log('enterCaptchaSolutions (datadome)', {
                    solutionNum: effectiveSolutions.length
                });

                if (!effectiveSolutions.length) {
                    result.error = 'No solutions provided';
                    return result;
                }

                for (const solution of effectiveSolutions) {
                    try {
                        const payload = typeof solution.payload === 'string'
                            ? JSON.parse(solution.payload)
                            : solution.payload;
                        const vendor = solution.vendor;

                        if (vendor !== 'DataDome') {
                            result.solved.push({
                                vendor: 'DataDome',
                                id: solution && solution.id ? solution.id : undefined,
                                responseElement: false,
                                responseCallback: false,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: 'Not a DataDome solution'
                            });
                            continue;
                        }

                        if (!solution || !solution.id) {
                            result.solved.push({
                                vendor: 'DataDome',
                                id: undefined,
                                responseElement: false,
                                responseCallback: false,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: 'Invalid solution payload (missing id)'
                            });
                            continue;
                        }

                        // DataDome solution typically includes a cookie value
                        // The cookie can be in different fields depending on the provider
                        // Log the full payload for debugging
                        this.log('Full payload received', JSON.stringify(payload));

                        let cookie = payload && (payload.cookie || payload.datadome || payload.Cookie);

                        // Normalize the cookie string - handle escaped forward slashes from JSON
                        // (e.g., "Path=\/" becomes "Path=/")
                        if (cookie) {
                            cookie = cookie.replace(/\\\//g, '/');
                        }

                        if (!cookie) {
                            this.log('Payload received (no cookie found)', payload);
                            result.solved.push({
                                vendor: 'DataDome',
                                id: solution.id,
                                responseElement: false,
                                responseCallback: false,
                                isSolved: false,
                                solvedAt: new Date().toISOString(),
                                error: 'Missing cookie/datadome in payload. Keys: ' + (payload ? Object.keys(payload).join(', ') : 'null')
                            });
                            continue;
                        }

                        this.log('Cookie value', cookie);

                        // Set the datadome cookie
                        let cookieSet = false;
                        try {
                            // The cookie value from the provider may already be fully formatted
                            // (e.g., "datadome=xxx; Domain=.fnac.com; Path=/; ...")
                            // or just the token value
                            let cookieString = cookie;

                            // Check if the cookie is already formatted (starts with "datadome=")
                            if (!cookie.startsWith('datadome=')) {
                                // Just the token value, need to format it
                                const domain = window.location.hostname;
                                cookieString = 'datadome=' + cookie + '; path=/; domain=.' + domain + '; SameSite=Lax; Secure';
                            }

                            // Set the cookie directly
                            document.cookie = cookieString;
                            cookieSet = true;

                            this.log('Set datadome cookie', {
                                cookieString: cookieString.substring(0, 80) + '...',
                                wasPreformatted: cookie.startsWith('datadome=')
                            });
                        } catch (e) {
                            this.log('Error setting cookie', String(e));
                        }

                        // Also try to call any DataDome callback if present
                        let callbackCalled = false;
                        if (typeof window.__datadome_callback === 'function') {
                            try {
                                window.__datadome_callback(cookie);
                                callbackCalled = true;
                            } catch (e) {
                                this.log('Callback error', String(e));
                            }
                        }

                        // Find and paint the container
                        const container = document.querySelector('[data-datadome-id="' + solution.id + '"]') ||
                            document.getElementById(solution.id);
                        if (container) {
                            this._paintCaptchaSolved(container);
                        }

                        if (cookieSet) {
                            result.needsReload = true;
                        }

                        result.solved.push({
                            vendor: 'DataDome',
                            id: solution.id,
                            responseElement: cookieSet,
                            responseCallback: callbackCalled,
                            isSolved: cookieSet || callbackCalled,
                            solvedAt: new Date().toISOString()
                        });
                    } catch (e) {
                        result.solved.push({
                            vendor: 'DataDome',
                            id: solution && solution.id ? solution.id : undefined,
                            responseElement: false,
                            responseCallback: false,
                            isSolved: false,
                            solvedAt: new Date().toISOString(),
                            error: String(e)
                        });
                    }
                }
            } catch (e) {
                result.error = String(e);
                this.log('enterCaptchaSolutions (datadome) - ERROR', String(e));
            }

            return result;
        }
    }

    window.dataDomeScript = new DataDomeContentScript(opts);
}
