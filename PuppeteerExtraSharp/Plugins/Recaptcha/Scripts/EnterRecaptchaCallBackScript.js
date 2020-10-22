const result = (function () {
    if (typeof (___grecaptcha_cfg) !== 'undefined') {
        let cs = [];
        for (let id in ___grecaptcha_cfg.clients) {
            cs.push(id);
        }
        let res = cs.map(cid => {
            for (let p in ___grecaptcha_cfg.clients[cid]) {
                let c = {};
                cid >= 10000 ? c.version = 'V3' : c.version = 'V2';
                let path = "___grecaptcha_cfg.clients[" + cid + "]." + p;
                let pp = eval(path);
                if (typeof pp === 'object') {
                    for (let s in pp) {
                        let subpath = "___grecaptcha_cfg.clients[" + cid + "]." + p + "." + s;
                        let sp = eval(subpath);
                        if (sp && typeof sp === 'object' && sp.hasOwnProperty('sitekey') && sp.hasOwnProperty('size')) {
                            c.sitekey = eval(subpath + '.sitekey');
                            let cb = eval(subpath + '.callback');
                            if (cb == null) {
                                c.callback = null;
                                c.function = null;
                            }
                            else {
                                c.callback = subpath + '.callback';
                                cb != c.callback ? c.function = cb : c.function = null;
                            }
                        }
                    }
                }
                return c;
            }
        });
        return (res)[0];
    } else {
        return (null);
    }
})()

if (typeof (result.function) == 'function') {
    result.function(value)
}

else {
    eval(result.function).call(window, value);
}
