namespace PuppeteerExtraSharpLite;

public static partial class Scripts {
    /// <summary>
    /// Overrides navigator.languages to return a frozen, custom list of language tags.
    /// </summary>
    /// <remarks>Depends on <see cref="Utils"/></remarks>
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
