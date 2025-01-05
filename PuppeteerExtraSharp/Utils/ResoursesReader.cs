using System.IO;
using System.Reflection;

namespace PuppeteerExtraSharp.Utils;

internal static class ResourcesReader
{
    public static string ReadFile(string path, Assembly customAssemly = null)
    {
        var assembly = customAssemly ?? Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(path);
        if (stream is null)
            throw new FileNotFoundException($"File with path {path} not found!");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
