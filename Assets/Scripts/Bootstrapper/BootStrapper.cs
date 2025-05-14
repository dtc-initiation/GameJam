using Managers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace Bootstrapper {
    public interface IBootService{
        Task InitializeAsync();
    }
    
    public static class ServiceRegistry{
        private static readonly Dictionary<Type, IBootService> services = new();

        public static void Register<T>(T service) where T : IBootService{
            services[typeof(T)] = service;
        }

        public static void Register(Type type, IBootService service){
            services[type] = service;
        }

        public static T Get<T>() where T : IBootService{
            return (T)services[typeof(T)];
        }
        
        public static void PrintRegisteredKeys()
        {
            Debug.Log("Registered services:");
            foreach (var key in services.Keys)
            {
                Debug.Log($"- {key.Name}");
            }
        }
        
    }

    public abstract class BootableMonoService : MonoBehaviour, IBootService
    {
        public abstract Task InitializeAsync();
    }
    
    public class BootStrapper : MonoBehaviour {
        public static bool IsBooted { get; private set; }
        [SerializeField] private List<BootableMonoService> monoServices;
        private readonly List<IBootService> nonMonoService = new ();
            
        async void Start(){
            Debug.Log("Boot: Starting Boot Sequence");

            await LoadConfigs();
            await RegisterServices();
            await InitializeServices();
            await LoadInitialScenes();

            IsBooted = true;
            Debug.Log("Boot : Game Ready");

        }

        async Task LoadConfigs(){
            Debug.Log("Boot : Loading Configs...");
            await Task.Delay(1000); // Simulate config loading
        }

        async Task InitializeServices(){
            Debug.Log("Boot : Initializing Services...");
            foreach (var service in monoServices)
            {
                await service.InitializeAsync();
            }
            foreach (var service in nonMonoService)
            {
                await service.InitializeAsync();
            }
        }
        
        async Task LoadInitialScenes(){
            Debug.Log("Boot : Loading Initial Scene...");
            SceneGroupManager sceneManager = ServiceRegistry.Get<SceneGroupManager>();
            await sceneManager.LoadSceneGroup("MainMenu");
        }

        async Task RegisterServices() {
            Debug.Log("Boot : Registering Services...");
            await RegisterMonoServices();
            await RegisterNonMonoServices();
            ServiceRegistry.PrintRegisteredKeys();
        }
        
        async Task RegisterMonoServices()
        {
            Debug.Log("Boot : Registering Mono Services...");
            foreach (var service in monoServices)
            {
                ServiceRegistry.Register(service.GetType(), service);
            }
            await Task.Delay(1000); // Simulate registration
        }
        
        async Task RegisterNonMonoServices(){
            Debug.Log("Boot : Registering Non-Mono Services...");
            await Task.Delay(1000); // Simulate registration
        }
    }
}

