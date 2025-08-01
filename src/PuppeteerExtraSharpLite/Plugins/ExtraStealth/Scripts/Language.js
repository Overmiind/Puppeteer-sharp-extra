(...languages) => {
    utils.replaceGetterWithProxy(
        Object.getPrototypeOf(navigator),
        'languages',
        utils.makeHandler().getterValue(Object.freeze(languages))
    )
}