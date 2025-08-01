(vendor) => {
    // Overwrite the `vendor` property to use a custom getter.
    utils.replaceGetterWithProxy(
        Object.getPrototypeOf(navigator),
        'vendor',
        utils.makeHandler().getterValue(vendor)
    )
}