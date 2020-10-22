using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using RestSharp;

namespace PuppeteerExtraSharp.Utils
{
    public static class RestHelper
    {
        public static IRestRequest AddQueryParameters(this IRestRequest request, Dictionary<string, string> parameters)
        {
            foreach (var (key,value) in parameters)
            {
                request.AddQueryParameter(key, value);
            }

            return request;
        }
    }
}
