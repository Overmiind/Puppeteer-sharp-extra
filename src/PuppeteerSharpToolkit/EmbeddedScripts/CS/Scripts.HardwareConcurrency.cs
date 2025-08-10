namespace PuppeteerSharpToolkit;

public static partial class Scripts {
    /// <summary>
    /// Overrides navigator.hardwareConcurrency with a custom value via a getter proxy.
    /// </summary>
    /// <remarks>Depends on <see cref="Utils"/></remarks>
    public const string HardwareConcurrency =
    """
    (concurrency) => {
        utils.replaceGetterWithProxy(
            Object.getPrototypeOf(navigator),
            'hardwareConcurrency',
            utils.makeHandler().getterValue(concurrency)
        )
    }
    //# sourceURL=HardwareConcurrency.js
    """;
}
