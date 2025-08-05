namespace PuppeteerExtraSharpLite;

public static partial class Scripts {
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
