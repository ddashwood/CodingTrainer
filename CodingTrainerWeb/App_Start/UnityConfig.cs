using CodingTrainer.CodingTrainerWeb.Dependencies;
using CodingTrainer.CodingTrainerWeb.Users;
using CodingTrainer.CodingTrainerWeb.Controllers;
using CodingTrainer.CodingTrainerWeb.Hubs.Helpers;
using CodingTrainer.CSharpRunner.CodeHost;
using System;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.RegistrationByConvention;
using CodingTrainer.Repositories;
using CodingTrainer.CodingTrainerWeb.ActionFilters;

namespace CodingTrainer.CodingTrainerWeb
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();


            // NOTE: Default lifetime manager is a Transient lifetime manager -
            // a new object every time one is needed


            container.RegisterTypes(AllClasses.FromLoadedAssemblies(), WithMappings.FromMatchingInterface, WithName.Default);


            container.RegisterType<ICodingTrainerRepository, SqlCodingTrainerRepository>();
            container.RegisterType<IIdeServices, IdeServices>();
            container.RegisterType<ICodeRunner, CodeRunner>();
            container.RegisterType<IExceptionLogger, CodeRunnerLogger>(new PerResolveLifetimeManager());
            container.RegisterType<IUserServices, UserServices>(new ContainerControlledLifetimeManager());

            // Controllers with more than one constructor, where
            // we want the one with fewer parameters to be used
            container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<ManageController>(new InjectionConstructor(new Type[] { typeof(IUserServices) }));

            // Action Filters aren't created using Unity's resolver, so we need to inject dependencies into them
            AuthorizeExerciseAttribute.UserServices = container.Resolve<IUserServices>();
            AuthorizeExerciseAttribute.DbRepository = container.Resolve<ICodingTrainerRepository>();
            LogAndHandleErrorAttribute.Repository = container.Resolve<ICodingTrainerRepository>();
        }
    }
}