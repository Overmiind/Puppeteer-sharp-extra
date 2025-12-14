// DataDome Interceptor Script
// Injected early to capture DataDome initialization and parameters

window.__datadome_captured = false;
window.__datadome_captcha_url = null;
window.__datadome_cid = null;
window.__datadome_hash = null;
window.__datadome_t = null;
window.__datadome_s = null;
window.__datadome_referer = null;
window.__datadome_callback = null;

// Intercept DataDome's dd property if it exists
let _originalDd = window.dd;
Object.defineProperty(window, 'dd', {
    get: function() {
        return _originalDd;
    },
    set: function(value) {
        _originalDd = value;

        // Try to extract parameters from dd object
        if (value && typeof value === 'object') {
            if (value.cid) window.__datadome_cid = value.cid;
            if (value.hsh || value.hash) window.__datadome_hash = value.hsh || value.hash;
            if (value.t) window.__datadome_t = value.t;
            if (value.s) window.__datadome_s = value.s;
            window.__datadome_captured = true;
        }
    },
    configurable: true
});

// Intercept ddjskey (DataDome JS key)
let _ddjskey = null;
Object.defineProperty(window, 'ddjskey', {
    get: function() {
        return _ddjskey;
    },
    set: function(value) {
        _ddjskey = value;
        window.__datadome_ddjskey = value;
    },
    configurable: true
});

// Monitor for DataDome captcha iframe creation
const originalCreateElement = document.createElement.bind(document);
document.createElement = function(tagName) {
    const element = originalCreateElement(tagName);

    if (tagName.toLowerCase() === 'iframe') {
        // Set up a MutationObserver to watch for src changes
        const observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(mutation) {
                if (mutation.type === 'attributes' && mutation.attributeName === 'src') {
                    const src = element.src;
                    if (src && (src.includes('geo.captcha-delivery.com') ||
                               src.includes('datadome') ||
                               src.includes('captcha-delivery'))) {
                        window.__datadome_captcha_url = src;

                        // Parse URL parameters
                        try {
                            const url = new URL(src);
                            const cid = url.searchParams.get('cid');
                            const hash = url.searchParams.get('hash') || url.searchParams.get('hsh');
                            const t = url.searchParams.get('t');
                            const s = url.searchParams.get('s');
                            const referer = url.searchParams.get('referer');

                            if (cid) window.__datadome_cid = cid;
                            if (hash) window.__datadome_hash = hash;
                            if (t) window.__datadome_t = t;
                            if (s) window.__datadome_s = s;
                            if (referer) window.__datadome_referer = referer;

                            window.__datadome_captured = true;
                        } catch (e) {
                            // URL parsing failed, ignore
                        }
                    }
                }
            });
        });

        observer.observe(element, { attributes: true });
    }

    return element;
};

// Intercept XHR to capture DataDome API calls
const originalXHROpen = XMLHttpRequest.prototype.open;
XMLHttpRequest.prototype.open = function(method, url) {
    if (url && (url.includes('datadome') || url.includes('captcha-delivery'))) {
        // Store the URL for parameter extraction
        this._datadome_url = url;

        const originalOnLoad = this.onload;
        this.onload = function() {
            try {
                if (this._datadome_url) {
                    const parsedUrl = new URL(this._datadome_url, window.location.origin);
                    const cid = parsedUrl.searchParams.get('cid');
                    if (cid) window.__datadome_cid = cid;
                }
            } catch (e) { }

            if (originalOnLoad) {
                originalOnLoad.apply(this, arguments);
            }
        };
    }

    return originalXHROpen.apply(this, arguments);
};

// Intercept fetch to capture DataDome API calls
const originalFetch = window.fetch;
window.fetch = function(input, init) {
    const url = typeof input === 'string' ? input : (input && input.url);

    if (url && (url.includes('datadome') || url.includes('captcha-delivery'))) {
        try {
            const parsedUrl = new URL(url, window.location.origin);
            const cid = parsedUrl.searchParams.get('cid');
            const hash = parsedUrl.searchParams.get('hash') || parsedUrl.searchParams.get('hsh');

            if (cid) window.__datadome_cid = cid;
            if (hash) window.__datadome_hash = hash;

            window.__datadome_captcha_url = url;
            window.__datadome_captured = true;
        } catch (e) { }
    }

    return originalFetch.apply(this, arguments);
};
