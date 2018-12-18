using Microsoft.AspNet.SignalR.Hubs;
using Unity;

namespace CodingTrainer.CodingTrainerWeb
{
    internal class UnityHubActivator : IHubActivator
    {
        private IUnityContainer container;

        public UnityHubActivator(IUnityContainer container)
        {
            this.container = container;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            return (IHub)container.Resolve(descriptor.HubType);
        }
    }
}