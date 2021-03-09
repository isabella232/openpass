using System;
using System.Web;

namespace Criteo.IdController.Helpers
{
    public static class HttpExtensions
    {
        public static Uri AddQueryParameter(this Uri uri, string name, string value)
        {
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);
            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            var uriBuilder = new UriBuilder(uri)
            {
                Query = httpValueCollection.ToString()
            };

            return uriBuilder.Uri;
        }
    }
}
