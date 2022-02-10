using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Models
{
    public class ResponseCacheEntry<T>
    {
        public string Id { get; set; }

        public DateTime CachedOn { get; set; } = DateTime.MinValue;

        public T Data { get; set; }

        private readonly TimeSpan _timeout = TimeSpan.FromHours(1);

        public bool Refreshing { get; set; }

        public bool Expired()
        {
            return DateTime.Now > CachedOn.Add(_timeout);
        }
    }

    public class ResponseCache<T>
    {
        private Dictionary<string, ResponseCacheEntry<T>> _cacheItems = new Dictionary<string, ResponseCacheEntry<T>>();

        private Dictionary<string, Func<T>> _cacheRefreshActions = new Dictionary<string, Func<T>>();

        private Timer _timer;

        public ResponseCache()
        {
            _timer = new Timer(new TimerCallback(TimerElapsed), null, 0, 1000);
        }

        public void Register(string itemKey, Func<T> refreshFunc)
        {
            if (_cacheRefreshActions.ContainsKey(itemKey) || _cacheItems.ContainsKey(itemKey))
            {
                return;
            }

            _cacheItems.Add(itemKey, new ResponseCacheEntry<T>()
            {
                Id = itemKey,
                Data = default(T),
                Refreshing = false,
                CachedOn = DateTime.MinValue
            });

            _cacheRefreshActions.Add(itemKey, refreshFunc);
        }

        public async Task<T> Get(string itemKey)
        {
            while (_cacheItems[itemKey].Data == null)
            {
                await Task.Delay(500);
            }

            return _cacheItems[itemKey].Data;
        }

        private void TimerElapsed(object state)
        {
            foreach (KeyValuePair<string, Func<T>> item in _cacheRefreshActions)
            {
                if (_cacheItems[item.Key].Expired() && !_cacheItems[item.Key].Refreshing)
                {
                    _cacheItems[item.Key].Refreshing = true;

                    Task.Factory.StartNew(() =>
                    {
                        _cacheItems[item.Key].Data = item.Value();

                        _cacheItems[item.Key].Refreshing = false;
                        _cacheItems[item.Key].CachedOn = DateTime.Now;
                    });
                }
            }
        }
    }
}
