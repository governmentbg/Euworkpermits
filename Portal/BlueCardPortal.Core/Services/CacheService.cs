using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using BlueCardPortal.Infrastructure.Model.Application;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Core.Services
{
    public class CacheService: ICacheService
    {
        private readonly IMemoryCache cache;
        public CacheService(IMemoryCache _cache)
        {
            cache = _cache;
        }
        public T GetValue<T>(string key) where T : class
        {
            var json = string.Empty;
            if (cache.TryGetValue(key, out json)){
                return JsonConvert.DeserializeObject<T>(json!)!;
            }
            return null!;
        }

        public void SetValue(string key, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            cache.Set(key, json, TimeSpan.FromHours(1));
        }
    }
}
