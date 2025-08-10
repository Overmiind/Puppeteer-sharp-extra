namespace PuppeteerExtraSharpLite;

public static partial class Scripts {
    /// <summary>
    /// Overrides navigator.vendor to return a custom vendor string via a getter proxy.
    /// </summary>
    /// <remarks>Depends on <see cref="Utils"/></remarks>
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