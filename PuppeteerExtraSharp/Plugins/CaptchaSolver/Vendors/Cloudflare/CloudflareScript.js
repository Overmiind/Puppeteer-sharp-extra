(opts) => {
    class CloudflareTurnstileContentScript {
        constructor(opts)
        {
            this.opts = opts;

            // Bind log if debug mode is provided
            this.log = (message, data) => {
                if (opts.debug) {
                    console.log(`[Turnstile] ${message}`, data);
                }
            };

            // Workaround for https://github.com/esbuild-kit/tsx/issues/113
            if (typeof globalThis.__name === "undefined") {
                globalThis.__defProp = Object.defineProperty;
                globalThis.__name = (target, value) =>
                    globalThis.__defProp(target, "name", {value, configurable: true});
            }

            this.log("Initialized", {url: document.location.href, opts: this.opts});
        }

        /** Check if element is visible */
        _isVisible(elem)
        {
            return !!(
                elem.offsetWidth ||
                elem.offsetHeight ||
                (typeof elem.getClientRects === "function" &&
                    elem.getClientRects().length)
            );
        }

        /** Check if element is in viewport */
        _isInViewport(elem)
        {
            if (!elem) return false;
            const rect = elem.getBoundingClientRect();
            return (
                rect.top >= 0 &&
                rect.left >= 0 &&
                rect.bottom <=
                (window.innerHeight ||
                    document.documentElement.clientHeight) &&
                rect.right <=
                (window.innerWidth ||
                    document.documentElement.clientWidth)
            );
        }

        /** Coloring iframe when busy */
        _paintCaptchaBusy($iframe)
        {
            try {
                if (this.opts.visualFeedback) {
                    $iframe.style.filter = "opacity(60%) hue-rotate(400deg)"; // violet
                }
            } catch (e) {
            }
            return $iframe;
        }

        /** Coloring iframe when solved */
        _paintCaptchaSolved($iframe)
        {
            try {
                if (this.opts.visualFeedback) {
                    $iframe.style.filter = "opacity(60%) hue-rotate(230deg)"; // green
                }
            } catch (e) {
            }
            return $iframe;
        }

        async _waitUntilDocumentReady()
        {
            return new Promise((resolve) => {
                if (!document || !window) return resolve(null);
                const loaded = /^loaded|^i|^c/.test(document.readyState);
                if (loaded) return resolve(null);

                const onReady = () => {
                    resolve(null);
                    document.removeEventListener("DOMContentLoaded", onReady);
                    window.removeEventListener("load", onReady);
                };

                document.addEventListener("DOMContentLoaded", onReady);
                window.addEventListener("load", onReady);
            });
        }

        /** Extract info from widget DOM */
        _extractWidgetInfo(widgetElement)
        {
            const sitekey = widgetElement.getAttribute("data-sitekey");
            if (!sitekey) return null;

            const action = widgetElement.getAttribute("data-action");
            const theme = widgetElement.getAttribute("data-theme");
            const size = widgetElement.getAttribute("data-size") || "normal";
            const isInvisible = size === 'invisible';

            return {
                vendor: "cloudflare",
                url: document.location.href,
                id: widgetElement.getAttribute("data-sitekey"), // unique enough
                sitekey,
                display: {action, theme, size},
                isInViewport: this._isInViewport(widgetElement),
                captchaType: isInvisible ? 'invisible' : 'checkbox',
                hasActiveChallengePopup: false // Turnstile does not expose popup iframes
            };
        }

        /** Locate Turnstile widgets */
        _findWidgets()
        {
            const nodes = document.querySelectorAll("[data-sitekey][data-cf-behavior], .cf-turnstile");
            return Array.from(nodes);
        }

        /** Locate iframes inside widget container */
        _findIframes(widgetElement)
        {
            return Array.from(widgetElement.querySelectorAll("iframe[src*='challenges.cloudflare.com']"));
        }

        /** Public: find captchas */
        async findCaptchas()
        {
            const result = {captchas: [], error: null};

            try {
                await this._waitUntilDocumentReady();

                const widgets = this._findWidgets();
                if (!widgets.length) return result;

                result.captchas = widgets
                    .map((el) => this._extractWidgetInfo(el))
                    .filter((info) => !!info && !!info.sitekey);

                widgets.forEach((widget) =>
                    this._findIframes(widget).forEach((frame) =>
                        this._paintCaptchaBusy(frame)
                    )
                );
            } catch (e) {
                result.error = e;
            }

            this.log("findTurnstiles result", result);
            return result;
        }

        /** Public: enter captcha solutions */
        async enterCaptchaSolutions(solutions)
        {
            const result = {solved: [], error: null};

            try {
                await this._waitUntilDocumentReady();

                if (!Array.isArray(solutions) || !solutions.length) {
                    result.error = "No solutions provided";
                    return result;
                }

                result.solved = solutions
                    .filter((s) => !!s.text)
                    .map((solution) => {
                        const widget = document.querySelector(
                            `[data-sitekey="${solution.id}"]`
                        );
                        if (!widget) {
                            return {
                                vendor: "cloudflare",
                                id: solution.id,
                                isSolved: false,
                                error: "Widget not found"
                            };
                        }

                        // Insert token into hidden input if present
                        const input =
                            widget.querySelector("input[name='cf-turnstile-response']") ||
                            document.querySelector("input[name='cf-turnstile-response']");

                        if (input) {
                            input.value = solution.text;
                            input.dispatchEvent(
                                new Event("input", {bubbles: true})
                            );
                            input.dispatchEvent(
                                new Event("change", {bubbles: true})
                            );
                        }

                        // Try calling JS callback if exists
                        const cbName = widget.getAttribute("data-callback");
                        if (cbName && typeof window[cbName] === "function") {
                            try {
                                window[cbName](solution.text);
                            } catch (e) {
                                this.log("Callback error", e);
                            }
                        }

                        // Color iframe
                        this._findIframes(widget).forEach((frame) =>
                            this._paintCaptchaSolved(frame)
                        );

                        return {
                            vendor: "cloudflare",
                            id: solution.id,
                            isSolved: true,
                            solvedAt: new Date().toISOString(),
                        };
                    });
            } catch (e) {
                result.error = e;
            }

            this.log("enterTurnstileSolutions result", result);
            return result;
        }
    }

    window.cfScript = new CloudflareTurnstileContentScript(opts);
}
