using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codit.AspNetCore.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class SessionTokenCache
    {
        private static readonly object FileLock = new object();
        private readonly string _cacheId;
        private readonly IMemoryCache _memoryCache;
        private TokenCache _cache = new TokenCache();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memoryCache"></param>
        public SessionTokenCache(string userId, IMemoryCache memoryCache)
        {
            // not object, we want the SUB
            _cacheId = userId + "_TokenCache";
            _memoryCache = memoryCache;

            Load();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TokenCache GetCacheInstance()
        {
            _cache.SetBeforeAccess(BeforeAccessNotification);
            _cache.SetAfterAccess(AfterAccessNotification);
            Load();

            return _cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SaveUserStateValue(string state)
        {
            lock (FileLock)
            {
                _memoryCache.Set(_cacheId + "_state", Encoding.ASCII.GetBytes(state));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ReadUserStateValue()
        {
            string state;
            lock (FileLock)
            {
                state = Encoding.ASCII.GetString(_memoryCache.Get(_cacheId + "_state") as byte[]);
            }

            return state;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            lock (FileLock)
            {
                _cache.Deserialize(_memoryCache.Get(_cacheId) as byte[]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Persist()
        {
            lock (FileLock)
            {
                // reflect changes in the persistent store
                _memoryCache.Set(_cacheId, _cache.Serialize());
                // once the write operation took place, restore the HasStateChanged bit to false
                _cache.HasStateChanged = false;
            }
        }

        /// <summary>
        /// Empties the persistent store.
        /// </summary>
        
        public void Clear()
        {
            _cache = null;
            lock (FileLock)
            {
                _memoryCache.Remove(_cacheId);
            }
        }

        
        
        /// <summary>
        /// Triggered right before MSAL needs to access the cache.
        /// Reload the cache from the persistent store in case it changed since the last access.
        /// </summary>
        /// <param name="args"></param>
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load();
        }

        /// <summary>
        /// Triggered right after MSAL accessed the cache.
        /// </summary>
        /// <param name="args"></param>
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (_cache.HasStateChanged)
            {
                Persist();
            }
        }
    }
}
