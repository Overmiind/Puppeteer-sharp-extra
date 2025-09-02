using System.Text.Json;

namespace Extra.Tests.Utils;

public static class JsonElementExtensions
{
    public static string GetString(this JsonElement element, string key)
    {
        return element.GetProperty(key).GetString();
    }
    
    public static bool GetBoolean(this JsonElement element, string key)
    {
        return element.GetProperty(key).GetBoolean();
    }
}