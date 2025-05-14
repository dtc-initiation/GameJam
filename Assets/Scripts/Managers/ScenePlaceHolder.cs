using System;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bootstrapper;

namespace Managers
{
    [Serializable]
    public class SceneGroup{
        public string groupName = "NewSceneGroup";
        public List<SceneData> scenes;
        public string FindSceneNameByType(SceneType sceneType) {
            return scenes.FirstOrDefault(scene => scene.sceneType == sceneType)?.sceneReference.Name;
        }
    }
    
    [Serializable]
    public class SceneData
    {
        public SceneReference sceneReference;
        public string Name => sceneReference.Name;
        public SceneType sceneType;
    }

    public enum SceneType
    {
        MainMenu, Levels, UI, HUD, Cinematic
    }
    
    public class ScenePlaceHolder{
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action<string> OnSceneGroupLoaded = delegate { };
        public event Action<string> OnSceneGroupUnLoaded = delegate { };
        private Dictionary<string, List<string>> LoadedScenes = new Dictionary<string, List<string>>();
        
        public async Task LoadScenes(SceneGroup sceneGroup)
        {
            OnSceneGroupLoaded.Invoke(sceneGroup.groupName);
            
            int toLoad = sceneGroup.scenes.Count;
            List<int> loadIndex = new List<int>();
            
            for (int i = 0; i < toLoad; i++)
            {
                var scene = sceneGroup.scenes[i];
                if (!IsLoaded(sceneGroup, scene))
                {
                    if (!LoadedScenes.ContainsKey(sceneGroup.groupName))
                    {
                        LoadedScenes.Add(sceneGroup.groupName, new List<string>());
                    }
                    LoadedScenes[sceneGroup.groupName].Add(scene.Name);
                    loadIndex.Add(i);
                }
            }
            var asyncOperationGroup = new AsyncOperationGroup(loadIndex.Count);
            foreach (var index in loadIndex)
            {
                var scene = sceneGroup.scenes[index];
                var asyncOperation = SceneManager.LoadSceneAsync(scene.sceneReference.Path, LoadSceneMode.Additive);
                asyncOperationGroup.Operations.Add(asyncOperation);
                OnSceneLoaded.Invoke(scene.Name);
            }

            while (!asyncOperationGroup.isDone){
                await Task.Delay(100);
            }
        }

        public async Task UnLoadScenes(SceneGroup sceneGroup)
        {
            OnSceneGroupUnLoaded.Invoke(sceneGroup.groupName);
            
            int toUnload = sceneGroup.scenes.Count;
            List<int> unloadIndex = new List<int>();

            for (int i = 0; i < toUnload; i++)
            {
                var scene = sceneGroup.scenes[i];
                if (IsLoaded(sceneGroup, scene))
                {
                    LoadedScenes[sceneGroup.groupName].Remove(scene.Name);
                    unloadIndex.Add(i);
                }
            }
            var asyncOperationGroup = new AsyncOperationGroup(unloadIndex.Count);     
            foreach (var index in unloadIndex)
            {
                var scene = sceneGroup.scenes[index];
                var asyncOperation = SceneManager.UnloadSceneAsync(scene.sceneReference.Path);
                asyncOperationGroup.Operations.Add(asyncOperation);
                OnSceneLoaded.Invoke(scene.Name);
            }
            
            while (!asyncOperationGroup.isDone){
                await Task.Delay(100);
            }
            
        }

        public bool IsLoaded(SceneGroup sceneGroup, SceneData sceneData)
        {
            bool isLoaded;
            if (LoadedScenes.ContainsKey(sceneGroup.groupName)){
                var loadedScenes = LoadedScenes[sceneGroup.groupName];
                isLoaded = loadedScenes.Contains(sceneData.Name);
            }
            else {isLoaded = false;}
            return isLoaded;
        }
        
    }
    
    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;
        public float progress => Operations.Count == 0 ? 0 : Operations.Sum(o => o.progress);
        public float percentage => progress / Operations.Count;
        public bool isDone => Operations.All(o => o.isDone);

        public AsyncOperationGroup(int capacity)
        {
            Operations = new List<AsyncOperation>(capacity);
        }
    }
    
}