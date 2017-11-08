using System.Collections.Generic;

namespace HeroesAggregator
{
    public static class PrimitiveCache
    {
        private static Dictionary<string, object> _cache = new Dictionary<string, object>();

        /// <summary>
        ///     Fetch an item from the cache as the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T FetchItem<T>(string key)
        {
            if (_cache.ContainsKey(key))
                return (T)_cache[key];

            return default(T);
        }

        /// <summary>
        ///     Add the item to the cache. Will overwrite existing data if key already exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        public static void AddOrUpdateItem(string key, object item) => _cache[key] = item;

        /// <summary>
        ///     If the given key is present in the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasKey(string key) => _cache.ContainsKey(key);
    }
}