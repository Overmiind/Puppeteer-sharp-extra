namespace PuppeteerExtraSharpLite.Plugins.EmbeddedScripts.CS;

public static partial class Scripts {
    public static ReadOnlySpan<char> Language =>
    """
    (...languages) => {
        utils.replaceGetterWithProxy(
            Object.getPrototypeOf(navigator),
            'languages',
            utils.makeHandler().getterValue(Object.freeze(languages))
        )
    }
    """;
}
