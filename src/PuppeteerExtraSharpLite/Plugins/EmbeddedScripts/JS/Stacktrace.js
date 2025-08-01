() => {
    const errors = {
        Error,
        EvalError,
        RangeError,
        ReferenceError,
        SyntaxError,
        TypeError,
        URIError
    }
    for (const name in errors) {
        // eslint-disable-next-line
        globalThis[name] = (function (NativeError) {
            return function (message) {
                const err = new NativeError(message)
                const stub = {
                    message: err.message,
                    name: err.name,
                    toString: () => err.toString(),
                    get stack() {
                        const lines = err.stack.split('\n')
                        lines.splice(1, 1) // remove anonymous function above
                        lines.pop() // remove puppeteer line
                        return lines.join('\n')
                    }
                }
                // eslint-disable-next-line
                if (this === globalThis) {
                    // called as function, not constructor
                    // eslint-disable-next-line
                    stub.__proto__ = NativeError
                    return stub
                }
                Object.assign(this, stub)
                // eslint-disable-next-line
                this.__proto__ = NativeError
            }
        })(errors[name])
    }
}