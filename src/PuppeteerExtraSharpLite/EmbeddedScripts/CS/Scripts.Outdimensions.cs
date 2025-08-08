namespace PuppeteerExtraSharpLite;

public static partial class Scripts {
    /// <summary>
    /// Ensures window.outerWidth and window.outerHeight are present by deriving sensible values
    /// from inner dimensions to avoid headless-only fingerprints.
    /// </summary>
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
