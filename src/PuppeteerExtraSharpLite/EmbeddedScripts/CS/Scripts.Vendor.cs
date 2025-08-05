namespace PuppeteerExtraSharpLite;

public static partial class Scripts {
    public const string Vendor =
    """
    (vendor) => {
        // Overwrite the `vendor` property to use a custom getter.
        utils.replaceGetterWithProxy(
            Object.getPrototypeOf(navigator),
            'vendor',
            utils.makeHandler().getterValue(vendor)
        )
    }
    //# sourceURL=Vendor.js
    """;
}