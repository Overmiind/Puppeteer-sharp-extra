using System.Collections.Generic;
using RestSharp;

namespace PuppeteerExtraSharp.Utils
{
    public static class RestHelper
    {
        public static RestRequest AddQueryParameters(this RestRequest request, Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                request.AddQueryParameter(parameter.Key, parameter.Value);
            }

            return request;
        }
    }
}
