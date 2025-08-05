namespace PuppeteerExtraSharpLite.Plugins.EmbeddedScripts.CS;

public static partial class Scripts {
    public static ReadOnlySpan<char> Outdimensions =>
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
    """;
}
