using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Bootstrapper {
    public static class ServiceLocator{
        private static readonly Dictionary<Type, object> services = new();
        public static void Register<T>(T instance) => services[typeof(T)] = instance;
        public static T Get<T>() => (T)services[typeof(T)];

    }

    public class BootStrapper : MonoBehaviour {
        public static bool IsBooted { get; private set; }
        
        async void Start(){
            Debug.Log("Boot: Starting Boot Sequence");

            await RegisterServices();
            await LoadConfigs();
            await InitializeServices();
            await LoadInitialScenes();

            IsBooted = true;
            Debug.Log("Boot : Game Ready");

        }

        async Task RegisterServices(){
            Debug.Log("Boot : Registering Services...");
        }

        async Task LoadConfigs(){
            Debug.Log("Boot : Loading Configs...");
        }

        async Task InitializeServices(){
            Debug.Log("Boot : Initializing Services...");
        }
        
        async Task LoadInitialScenes(){
            Debug.Log("Boot : Loading Initial Scene...");
        }
    }
}

