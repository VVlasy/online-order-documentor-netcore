using online_order_documentor_netcore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Providers
{
    public class CacheProvider
    {
        private Dictionary<string, ResponseCache<object>> _cacheKeys = new Dictionary<string, ResponseCache<object>>();

        public ResponseCache<T> GetCache<T>(string key) 
        {
            if (!_cacheKeys.ContainsKey(key))
            {
                _cacheKeys.Add(key, new ResponseCache<T>() as ResponseCache<object>);
            }
            
            return _cacheKeys[key] as ResponseCache<T>;
        }
    }
}
