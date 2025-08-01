using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace PuppeteerExtraSharpLite.Utils;

internal static class ResourcesReader
{
    public static string ReadFile(ReadOnlySpan<byte> resource)
    {
        using var ms = new MemoryStream();
        using var cs = new GZipStream(ms, CompressionMode.Decompress);
        ms.Write(resource);
        ms.Position = 0;
        
        var buffer = new byte[1024];
        using var memoryStream = new MemoryStream();
        int count;
        while ((count = cs.Read(buffer, 0, buffer.Length)) > 0)
        {
            memoryStream.Write(buffer, 0, count);
        }
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
}