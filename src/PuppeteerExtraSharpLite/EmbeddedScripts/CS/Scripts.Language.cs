namespace PuppeteerExtraSharpLite;

public static partial class Scripts {
    public const string Language =
    """
    (...languages) => {
        utils.replaceGetterWithProxy(
            Object.getPrototypeOf(navigator),
            'languages',
            utils.makeHandler().getterValue(Object.freeze(languages))
        )
    }
    //# sourceURL=Language.js
    """;
}
