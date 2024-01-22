using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        public int HandleCount { get; private set; } = 0;

        internal IReadOnlyDictionary<string, UObject> Results => m_results;

        // for cache (pooling)
        readonly Dictionary<string, UObject> m_results = new();

        // async handles with addressable asset key
        readonly Dictionary<string, AsyncOperationHandle> _handles = new();


        // label - list<string> used for release with label
        readonly Dictionary<string, List<string>> _labels = new(); // dependent with _results

        /// <summary>
        /// Get cached UnityObject as type T. Returns null if there is no cached object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key) where T : UObject
        {
            if (Results.ContainsKey(key) == true)
            {
                T result = Results[key] as T;
#if DEBUG
                if (result == null)
                {
                    Debug.LogError($"Can not cast UnityObject to {typeof(T)}");
                }
#endif

                return result;
            }
            else
            {
#if DEBUG
                Debug.LogError($"There is no cached addressable assets. [key={key}]");
#endif
                return null;
            }
        }

        /// <summary>
        /// Load addressable asset and invoke the callback with it as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void LoadAsync<T>(string key, Action<T> callback = null) where T : UObject
        {
            // cached -> already done!
            if (m_results.TryGetValue(key, out UObject resource))
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
                    m_results.Add(key, opHandle.Result as UObject);
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

        public void LoadWithLabelAsync(string label, Action<List<string>> completed = null)
        {
            if (_labels.ContainsKey(label))
            {
#if DEBUG
                Debug.Log($"All assets are already loaded or loading now. [label={label}]");
#endif
                return;
            }


            // _labels에 key가 없음
            _labels.Add(label, new List<string>());
            Addressables.LoadResourceLocationsAsync(label, typeof(UObject)).Completed += OnLocationsLoaded;

            void OnLocationsLoaded(AsyncOperationHandle<IList<IResourceLocation>> opHandle)
            {
                if (opHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    IList<IResourceLocation> locations = opHandle.Result;
                    string[] foundKeys = new string[locations.Count];

                    for (int i=0; i<locations.Count; ++i)
                    {
                        foundKeys[i] = locations[i].PrimaryKey;
                        _labels[label].Add(locations[i].PrimaryKey);
                    }
                    LoadAllAsync(completed, foundKeys);
                }
                else
                {
#if DEBUG
                    Debug.LogError($"Failed to load addressables [label={label}]");
#endif
                }
            }
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="completed">The successfully completed count of the keys</param>
        /// <param name="keys">Addressable keys to load GameObject</param>
        public void LoadAllAsync(Action<List<string>> completed, params string[] keys)
        {
            if (keys == null || keys.Length == 0)
            {
                completed?.Invoke(new List<string>());
                return;
            }

            List<string> failedKey = new List<string>();
            int completedCount = 0;

            foreach (string key in keys)
            {
                void OnHandleCompleted(AsyncOperationHandle handle)
                {
                    HandleCount--;
                    completedCount++;
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        m_results.Add(key, handle.Result as UObject);
                    }

                    if (completedCount >= keys.Length)
                    {
                        foreach (string key in keys)
                        {
                            if (_handles.TryGetValue(key, out var h) == true)
                            {
                                if (h.Status != AsyncOperationStatus.Succeeded)
                                {
                                    Debug.LogError("Failed to load addressable prefab: " + h.OperationException);
                                    failedKey.Add(key);
                                }
                            }
                        }

                        // 모든 로드가 완료되면 콜백 실행
                        completed?.Invoke(failedKey);
                    }
                }

                // 이미 로드 되어 있는지 확인
                if (m_results.ContainsKey(key) == true)
                {
                    completedCount++;
                    continue;
                }

                // _handle에 핸들 추가
                // 이미 로드 중인지 확인
                if (_handles.ContainsKey(key) == true)
                {
                    _handles[key].Completed += OnHandleCompleted;
                    continue;
                }

                AsyncOperationHandle loadHandle = Addressables.LoadAssetAsync<UObject>(key);
                HandleCount++;
                loadHandle.Completed += OnHandleCompleted;
                _handles.Add(key, loadHandle);
            }
        }


        /// <summary>
        /// Load a resource with the key and release it after callback. (no caching)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void LoadAsyncOnce<T>(string key, Action<T> callback = null) where T : UObject
        {
            if (m_results.TryGetValue(key, out var resource))
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
                    m_results.Add(key, opHandle.Result as UObject);
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
            m_results.Clear();

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
            if (m_results.ContainsKey(key) == false)
            {
                return;
            }
            m_results.Remove(key);

            if (_handles.TryGetValue(key, out AsyncOperationHandle handle))
            {
                Addressables.Release(handle);
            }

            _handles.Remove(key);
        }

        public void Release(params string[] keys)
        {
            foreach (var key in keys) Release(key);
        }

        public void ReleaseWithLabel(string label)
        {
            // Note: 성공적으로 로드 하지 못한 asset에 대한 key가 있을 수 있음
            // 해당 에셋에 대해서는 release해도 무관.
            if (_labels.TryGetValue(label, out var keys))
            {
                foreach (var key in keys) Release(key);
            }
            _labels.Remove(label);
        }

        /// <summary>
        /// Instantiate a prefab with adrressable key (as GameObject) NOTE: the name of instantiated object is the original name of the prefab.
        /// </summary>
        /// <param name="key">addressable key</param>
        /// <param name="parent">transform of parent</param>
        /// <param name="callback">action callback</param>
        public void InstantiateAsync(string key, Transform parent = null, Action<GameObject> callback = null)
        {
            LoadAsync<GameObject>(key, (prefab) =>
            {
                GameObject go = UObject.Instantiate(prefab, parent);
                go.name = prefab.name;
                go.transform.localPosition = prefab.transform.position;
                
                callback?.Invoke(go);
            });
        }

        public void InstantiateOnceAsync(string key, Transform parent = null, Action<GameObject> callback = null)
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

        public void DestroyAllChilds(Transform parent)
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


