using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ProjectS
{
    public class AssetLoader
    {
        #region Member
        
        public static AssetLoader Instance { get; } = new AssetLoader();
        private IDictionary<string, AsyncOperationHandle> Dict = new Dictionary<string, AsyncOperationHandle>();
        private IDictionary<string, IList<string>> loadedAssetNames = new Dictionary<string, IList<string>>();
        private AssetLoader() { }
        
        #endregion

        #region Method
        
        public async Task<IList<UnityEngine.Object>> Load(string label)
        {
            return await Load<UnityEngine.Object>(label);
        }
        
        public async Task<IList<T>> Load<T>(string label) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetsAsync<T>(label, obj => { });
            var objects = await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Dict.Add(label, handle);
                loadedAssetNames.Add(label, handle.Result.Select(t => t.name).ToArray());
                ResourceStore.Instance.Store(handle.Result);
            }
            else
            {
                Addressables.Release(handle);
            }
            
            return objects;
        }
        
        public void Unload(string label)
        {
            if (!Dict.ContainsKey(label)) return;
            
            var handle = Dict[label];
            Addressables.Release(handle);
            
            var names = loadedAssetNames[label];
            ResourceStore.Instance.Release(names);
            
            Dict.Remove(label);
            loadedAssetNames.Remove(label);
        }
        
        public bool IsLoadedSystemAsset
        {
            get
            {
                return loadedAssetNames.ContainsKey("System");
            }
        }
        
        public async Task<IList<UnityEngine.Object>> LoadSystem()
        {
            return await Load<UnityEngine.Object>("System");
        }
        
        #endregion
    }
}