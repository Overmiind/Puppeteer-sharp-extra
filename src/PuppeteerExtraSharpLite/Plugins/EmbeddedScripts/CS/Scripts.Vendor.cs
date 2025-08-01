namespace PuppeteerExtraSharpLite.Plugins.EmbeddedScripts.CS;

public static partial class Scripts {
    public static ReadOnlySpan<char> Vendor =>
    """
    (vendor) => {
        // Overwrite the `vendor` property to use a custom getter.
        utils.replaceGetterWithProxy(
            Object.getPrototypeOf(navigator),
            'vendor',
            utils.makeHandler().getterValue(vendor)
        )
    }
    """;
}