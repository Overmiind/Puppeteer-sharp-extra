using System;
using System.Collections.Generic;
using System.Linq;


namespace PuppeteerExtraSharp.Utils;

public static class QueryHelper
{
   public static string ToQuery(Dictionary<string, string> query)
   {
       if (query == null || query.Count == 0)
           return string.Empty;
           
       return string.Join("&", query.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
   }
}