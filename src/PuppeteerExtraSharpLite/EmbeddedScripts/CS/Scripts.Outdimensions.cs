namespace PuppeteerExtraSharpLite;

public static partial class Scripts {
    public const string Outdimensions =
    """
    (() => {
        try {
            if (window.outerWidth && window.outerHeight) {
                return // nothing to do here
            }
            const windowFrame = 85 // probably OS and WM dependent
            window.outerWidth = window.innerWidth
            window.outerHeight = window.innerHeight + windowFrame
        } catch (err) { }
    })();
    //# sourceURL=Outdimensions.js
    """;
}
