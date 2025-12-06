window.__geetestCaptured = false;
window.__geetestInstance = null;

// Intercepter initGeetest4 s'il est défini plus tard
let _initGeetest4 = null;
Object.defineProperty(window, 'initGeetest4', {
    get: function() {
        return _initGeetest4;
    },
    set: function(value) {
        _initGeetest4 = function(config, callback) {
            return value(config, function(captchaObj) {
                window.__geetestInstance = captchaObj;
                if (callback) callback(captchaObj);
            });
        };
    }
});
        