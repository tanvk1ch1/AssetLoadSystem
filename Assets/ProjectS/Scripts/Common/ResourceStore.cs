using System.Collections.Generic;
using UnityEngine;

namespace ProjectS
{
    public sealed class ResourceStore
    {
        #region Member
        
        public static ResourceStore Instance { get; } = new ResourceStore();
        private Dictionary<string, UnityEngine.Object> assetDict = new Dictionary<string, UnityEngine.Object>();
        private ResourceStore() { }
        
        #endregion

        #region Method
        
        public void Store<T>(IList<T> assets) where T : UnityEngine.Object
        {
            foreach (var obj in assets)
            {
                if (assetDict.ContainsKey(obj.name)) continue;
                assetDict.Add(obj.name, obj);
            }
        }

        public void Release(IList<string> assetName)
        {
            foreach (var name in assetName)
            {
                assetDict.Remove(name);
            }
        }

        public T Get<T>(string key) where T : UnityEngine.Object
        {
            try
            {
                var obj = assetDict[key];
                return (T) obj;
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogErrorFormat("key is {0}", key);
                throw e;
            }
        }
        
        #endregion
    }
}