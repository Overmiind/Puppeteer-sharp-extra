namespace PuppeteerExtraSharpLite;

public static partial class Scripts {
    /// <summary>
    /// Normalizes Notifications and Permissions APIs to reflect headful differences on secure
    /// and insecure origins (e.g., Notification.permission and permissions.query results).
    /// </summary>
    /// <remarks>Depends on <see cref="Utils"/></remarks>
    public const string Permissions =
    """
    (() => {
        const isSecure = document.location.protocol.startsWith('https');

        // In headful on secure origins the permission should be "default", not "denied"
        if (isSecure) {
            utils.replaceGetterWithProxy(Notification, 'permission', {
                apply() {
                    return 'default';
                }
            });
        }

        // Another weird behavior:
        // On insecure origins in headful the state is "denied",
        // whereas in headless it's "prompt"
        if (!isSecure) {
            const handler = {
                apply(target, ctx, args) {
                    const param = (args || [])[0];

                    const isNotifications =
                        param && param.name && param.name === 'notifications';
                    if (!isNotifications) {
                        return utils.cache.Reflect.apply(...arguments);
                    }

                    return Promise.resolve(
                        Object.setPrototypeOf(
                            {
                                state: 'denied',
                                onchange: null
                            },
                            PermissionStatus.prototype
                        )
                    );
                }
            };
            // Note: Don't use `Object.getPrototypeOf` here
            utils.replaceWithProxy(Permissions.prototype, 'query', handler);
        }
    })();
    //# sourceURL=Permissions.js
    """;
}