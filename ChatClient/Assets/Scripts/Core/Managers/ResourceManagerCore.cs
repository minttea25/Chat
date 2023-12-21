using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

using UObject = UnityEngine.Object;

namespace Core
{
    public class ResourceManagerCore : IManager
    {
        // 참고: 단순 Addressables.LoadAsset<>()은 더이상 사용되지 않음 (deprecated)
        // Async을 사용

        // Number of async-loadings in progress (로딩 완료 후 _handles에서 삭제안함, _handles.Count와 의미가 다름)
        public int HandleCount = 0;

        // for cache (pooling)
        readonly Dictionary<string, UObject> _resources = new();
        readonly Dictionary<string, UObject> _locResources = new();

        // async handles with addressable asset key
        readonly Dictionary<string, AsyncOperationHandle> _handles = new();

        // just for loading and caching
        public void LoadAsync<T>(string key, Action<T> callback = null) where T : UObject
        {
            // cached -> already done!
            if (_resources.TryGetValue(key, out UObject resource))
            {
                callback?.Invoke(resource as T);
                return;
            }

            // if the loading is on-going, add just the callback at handle and return
            if (_handles.ContainsKey(key))
            {
                _handles[key].Completed += (opHandle) => { callback?.Invoke(opHandle.Result as T); };
                return;
            }

            // load async
            _handles.Add(key, Addressables.LoadAssetAsync<T>(key));
            HandleCount++;
            _handles[key].Completed += (opHandle) =>
            {
                if (opHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _resources.Add(key, opHandle.Result as UObject);
                    callback?.Invoke(opHandle.Result as T);
                    HandleCount--;
                }
                else
                {
#if DEBUG
                    Debug.LogError($"Failed to load addressables [key={key}] as {typeof(T)}");
#endif
                    HandleCount--;
                }
            };
        }

        public void LoadAsync<T>(IResourceLocation location, Action<T> callback = null) where T : UObject
        {
            // cached -> already done!
            if (_locResources.TryGetValue(location.ToString(), out UObject resource))
            {
                callback?.Invoke(resource as T);
                return;
            }

            // if the loading is on-going, add just the callback at handle and return
            if (_handles.ContainsKey(location.ToString()))
            {
                _handles[location.ToString()].Completed += (opHandle) => { callback?.Invoke(opHandle.Result as T); };
                return;
            }

            // load async
            _handles.Add(location.ToString(), Addressables.LoadAssetAsync<T>(location));
            HandleCount++;
            _handles[location.ToString()].Completed += (opHandle) =>
            {
                _locResources.Add(location.ToString(), opHandle.Result as UObject);
                callback?.Invoke(opHandle.Result as T);
                HandleCount--;
            };
        }

        /// <summary>
        /// Load a resource with the key and release it after callback. (no caching)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void LoadAsyncOnce<T>(string key, Action<T> callback = null) where T : UObject
        {
            if (_resources.TryGetValue(key, out var resource))
            {
                callback?.Invoke(resource as T);
                return;
            }

            // if the loading is on-going, add just the callback at handle and return
            if (_handles.ContainsKey(key))
            {
                _handles[key].Completed += (opHandle) => { callback?.Invoke(opHandle.Result as T); };
                return;
            }

            // load async
            _handles.Add(key, Addressables.LoadAssetAsync<T>(key));
            HandleCount++;
            _handles[key].Completed += (opHandle) =>
            {
                if (opHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _resources.Add(key, opHandle.Result as UObject);
                    callback?.Invoke(opHandle.Result as T);
                    HandleCount--;
                }
                else
                {
#if DEBUG
                    Debug.LogError($"Failed to load addressables [key={key}] as {typeof(T)}");
#endif
                    HandleCount--;
                }

                Addressables.Release(_handles[key]);
                _handles.Remove(key);
            };
        }

        public void LoadAssetsAsyncOnce<T>(string label, Action finalCallback = null, Action<IList<T>> callback = null)
        {
            AsyncOperationHandle<IList<T>> loadHandle = Addressables.LoadAssetsAsync<T>(label, null);
            loadHandle.Completed += (opHandle) =>
            {
                if (opHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    IList<T> loadedData = opHandle.Result;
                    callback?.Invoke(loadedData);

                    // Check if this is the last loaded data
                    if (opHandle.OperationException == null && opHandle.Result != null && opHandle.Result.Count > 0)
                    {
                        // The last data is loaded
                        finalCallback?.Invoke();
                    }
                }
                else
                {
                    // Handle loading error
                    Debug.LogError("Failed to load multiple addressables: " + opHandle.OperationException);
                }

                Addressables.Release(loadHandle);
            };
        }

        /// <summary>
        /// Use this function to load an image for Sprite.
        /// </summary>
        /// <param name="key">Addressable key</param>
        /// <param name="callback">callback function called loaded</param>
        public void LoadImageAsync(string key, Action<Sprite> callback = null)
        {
            LoadAsync(key, callback);
        }

        public void LoadTexture2DAsync(string key, Action<Texture2D> callback = null)
        {
            LoadAsync(key, callback);
        }

        public void LoadAudioClipAsync(string key, Action<AudioClip> callback = null)
        {
            LoadAsync(key, callback);
        }

        public void Clear()
        {
            _resources.Clear();

            foreach (var handle in _handles.Values)
                Addressables.Release(handle);

            _handles.Clear();
        }

        /// Note: Even though caching is false, the object would be still cached if the object is once cached.
        

        /// <summary>
        /// release 이후에 _resources와 _handles에서 삭제 (캐싱 해제)
        /// </summary>
        /// <param name="key">key of addressable</param>
        public void Release(string key)
        {
            // if there is no object with the key in _resources, do nothing
            if (_resources.ContainsKey(key) == false)
            {
                return;
            }
            _resources.Remove(key);

            if (_handles.TryGetValue(key, out AsyncOperationHandle handle))
            {
                Addressables.Release(handle);
            }

            _handles.Remove(key);
        }

        /// <summary>
        /// release 이후에 _locResources와 _handles에서 삭제 (캐싱 해제)
        /// </summary>
        /// <param name="location"></param>
        public void Release(IResourceLocation location)
        {
            // if there is no object with the key in _resources, do nothing
            if (_locResources.TryGetValue(location.ToString(), out UObject _) == false)
            {
                return;
            }

            _locResources.Remove(location.ToString());

            if (_handles.TryGetValue(location.ToString(), out AsyncOperationHandle handle))
            {
                Addressables.Release(handle);
            }

            _handles.Remove(location.ToString());
        }

        /// <summary>
        /// Instantiate a prefab with adrressable key (as GameObject) NOTE: the name of instantiated object is the original name of the prefab.
        /// </summary>
        /// <param name="key">addressable key</param>
        /// <param name="parent">transform of parent</param>
        /// <param name="callback">action callback</param>
        public void Instantiate(string key, Transform parent = null, Action<GameObject> callback = null)
        {
            LoadAsync<GameObject>(key, (prefab) =>
            {
                GameObject go = UObject.Instantiate(prefab, parent);
                go.name = prefab.name;
                go.transform.localPosition = prefab.transform.position;
                
                callback?.Invoke(go);
            });
        }

        public void InstantiateOnce(string key, Transform parent = null, Action<GameObject> callback = null)
        {
            LoadAsyncOnce<GameObject>(key, (prefab) =>
            {
                GameObject go = UObject.Instantiate(prefab, parent);
                go.name = prefab.name;
                go.transform.localPosition = prefab.transform.position;

                callback?.Invoke(go);
            });
        }

        public void Destroy(GameObject go, float timeSeconds = 0.0f, bool release = false)
        {
            UObject.Destroy(go, timeSeconds);

            // NEED CHECK
            if (release)
            {
                Addressables.ReleaseInstance(go);
            }
        }

        public void DestroyAllItems(Transform parent)
        {
            foreach(Transform item in parent)
            {
                Destroy(item.gameObject);
            }
        }

        void IManager.InitManager()
        {
        }

        void IManager.ClearManager()
        {
            Clear();
        }
    }
}


