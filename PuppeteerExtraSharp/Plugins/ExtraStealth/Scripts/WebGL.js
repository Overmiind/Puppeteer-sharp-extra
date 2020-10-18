() => {const getParameter = WebGLRenderingContext.getParameter;
    WebGLRenderingContext.prototype.getParameter = function(parameter) {
        // UNMASKED_VENDOR_WEBGL
        if (parameter === 37445) {
            return 'Intel Open Source Technology Center';
        }
        // UNMASKED_RENDERER_WEBGL
        if (parameter === 37446) {
            return 'Mesa DRI Intel(R) Ivybridge Mobile ';
        }
        
        return getParameter(parameter);
    };}