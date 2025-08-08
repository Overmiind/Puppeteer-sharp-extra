namespace PuppeteerExtraSharpLite;

public static partial class Scripts {
    /// <summary>
    /// Patches navigator.webdriver to match headful Chrome behavior, removing the webdriver flag
    /// where applicable so automation is harder to detect.
    /// </summary>
    public const string WebDriver =
    """
    (() => {
        if (navigator.webdriver === false) {
            // Post Chrome 89.0.4339.0 and already good
        } else if (navigator.webdriver === undefined) {
            // Pre Chrome 89.0.4339.0 and already good
        } else {
            // Pre Chrome 88.0.4291.0 and needs patching
            delete Object.getPrototypeOf(navigator).webdriver
        }
    })();
    //# sourceURL=WebDriver.js
    """;
}