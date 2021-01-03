using System.Collections.Generic;
using RestSharp;

namespace PuppeteerExtraSharp.Utils
{
    public static class RestHelper
    {
        public static IRestRequest AddQueryParameters(this IRestRequest request, Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                request.AddQueryParameter(parameter.Key, parameter.Value);
            }

            return request;
        }
    }
}
