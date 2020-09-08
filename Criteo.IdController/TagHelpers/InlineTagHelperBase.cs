using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;

// Inspired by this code: https://www.meziantou.net/inlining-a-stylesheet-a-javascript-or-an-image-file-using-a-taghelper-in-asp-net.htm
namespace Criteo.IdController.TagHelpers
{
    public abstract class InlineTagHelperBase : TagHelper
    {
        private const string CacheKeyPrefix = "InlineTagHelper-";

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMemoryCache _cache;

        protected InlineTagHelperBase(IHostingEnvironment hostingEnvironment, IMemoryCache cache)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
        }

        private async Task<T> GetContentAsync<T>(ICacheEntry entry, string path, Func<IFileInfo, Task<T>> getContent)
        {
            var fileProvider = _hostingEnvironment.WebRootFileProvider;
            var changeToken = fileProvider.Watch(path);

            entry.SetPriority(CacheItemPriority.NeverRemove);
            entry.AddExpirationToken(changeToken);

            var file = fileProvider.GetFileInfo(path);
            if (file == null || !file.Exists)
                return default(T);

            return await getContent(file);
        }

        protected Task<string> GetFileContentAsync(string path)
        {
            return _cache.GetOrCreateAsync(CacheKeyPrefix + path, entry => GetContentAsync(entry, path, ReadFileContentAsStringAsync));
        }

        private static async Task<string> ReadFileContentAsStringAsync(IFileInfo file)
        {
            using (var stream = file.CreateReadStream())
            using (var textReader = new StreamReader(stream))
            {
                return await textReader.ReadToEndAsync();
            }
        }
    }
}
