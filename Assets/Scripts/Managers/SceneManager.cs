using System;
using UnityEngine;
using System.Threading.Tasks;
using Bootstrapper;

namespace Managers
{
    public class SceneGroupManager : BootableMonoService{
        public static ScenePlaceHolder Instance { get; private set; }
        
        [SerializeField] private SceneGroup[] sceneGroups;
        public readonly ScenePlaceHolder PlaceHolder = new ScenePlaceHolder();

        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public void Awake(){
            PlaceHolder.OnSceneLoaded += (sceneName) => { Debug.Log($"Scene Loaded: {sceneName}"); };
            PlaceHolder.OnSceneUnloaded += (sceneName) => { Debug.Log($"Scene Unloaded: {sceneName}"); };
            PlaceHolder.OnSceneGroupLoaded += (groupName) => { Debug.Log($"Scene Group Loaded: {groupName}"); };
            PlaceHolder.OnSceneGroupUnLoaded += (groupName) => { Debug.Log($"Scene Group Unloaded: {groupName}"); };
        }
        
        public async Task LoadSceneGroup(string groupName)
        {
            int index = GetGroupNameIndex(groupName);
            if (index != -1){await PlaceHolder.LoadScenes(sceneGroups[index]);}
            else {Debug.Log($"{groupName} does not exist in Scene List");}
            
        }
        
        public async Task UnloadSceneGroup(string groupName)
        {
            int index = GetGroupNameIndex(groupName);
            if (index != -1){await PlaceHolder.UnLoadScenes(sceneGroups[index]);}
            else {Debug.Log($"{groupName} does not exist in Loaded Scenes");}
        }

        public int GetGroupNameIndex(string groupName)
        {
            int index = -1;
            foreach (var group in sceneGroups)
            {
                if (group.groupName == groupName)
                {
                    index = Array.IndexOf(sceneGroups, group);
                }
            }
            return index;
        }
        
    }
}