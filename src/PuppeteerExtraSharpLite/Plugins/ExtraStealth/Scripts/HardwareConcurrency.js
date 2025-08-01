(concurrency) => {

    utils.replaceGetterWithProxy(
        Object.getPrototypeOf(navigator),
        'hardwareConcurrency',
        utils.makeHandler().getterValue(concurrency)
    )
}