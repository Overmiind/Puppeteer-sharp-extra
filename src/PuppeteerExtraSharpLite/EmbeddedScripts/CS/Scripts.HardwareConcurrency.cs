namespace PuppeteerExtraSharpLite;

public static partial class Scripts {
    public static ReadOnlySpan<char> HardwareConcurrency =>
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
