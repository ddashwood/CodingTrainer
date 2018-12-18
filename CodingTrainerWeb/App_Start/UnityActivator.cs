using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CodingTrainer.CodingTrainerWeb.UnityActivator), nameof(CodingTrainer.CodingTrainerWeb.UnityActivator.Start))]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(CodingTrainer.CodingTrainerWeb.UnityActivator), nameof(CodingTrainer.CodingTrainerWeb.UnityActivator.Shutdown))]

namespace CodingTrainer.CodingTrainerWeb
{
    /// <summary>
    /// Provides the bootstrapping for integrating Unity with ASP.NET MVC.
    /// </summary>
    public static class UnityActivator
    {
        private static UnityHubActivator unityHubActivator;

        /// <summary>
        /// Integrates Unity when the application starts.
        /// </summary>
        public static void Start()
        {
            // MVC
            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new Unity.AspNet.Mvc.UnityFilterAttributeFilterProvider(UnityConfig.Container));

            DependencyResolver.SetResolver(new Unity.AspNet.Mvc.UnityDependencyResolver(UnityConfig.Container));

            // Web API
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(UnityConfig.Container);

            // SignalR
            unityHubActivator = new UnityHubActivator(UnityConfig.Container);
            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => unityHubActivator);

            // TODO: Uncomment if you want to use PerRequestLifetimeManager
            // Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));
        }

        /// <summary>
        /// Disposes the Unity container when the application is shut down.
        /// </summary>
        public static void Shutdown()
        {
            UnityConfig.Container.Dispose();
        }
    }
}