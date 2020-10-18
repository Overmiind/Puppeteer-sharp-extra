() => {
    Object.defineProperty(Object.getPrototypeOf(navigator), 'languages', {
        get: () => ['en-US', 'en']
    })
}