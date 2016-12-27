using Messaging.Queue.Provider.Domain.Enum;
using Messaging.Queue.Provider.Domain.Service.Factories;
using Messaging.Queue.Provider.Domain.Service.Interfaces;
using Messaging.Queue.Provider.Domain.Service.Interfaces.Providers;
using Messaging.Queue.Provider.Infra.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Infra.ServiceBus.Factories
{
    public class ServiceBusProviderFactory : IServiceBusProviderFactory
    {
        #region Private Fields

        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructor

        public ServiceBusProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region IServiceBusProviderFactory Members

        public ISbQueue<T> Create<T>(Serializer serializer, string queueName) where T : class
        {
            var messageQueue = _serviceProvider.GetService(typeof(SbQueue<T>)) as ISbQueue<T>;

            if (messageQueue == null)
            {
                throw new Exception("ServiceProvider get 'IMessageQueue<T>' service error.");
            }

            switch (serializer)
            {
                case Serializer.Json:
                    messageQueue.MessageSerializer = _serviceProvider.GetService(typeof(JsonSerializer<T>)) as IMessageSerializer<T>;
                    break;

                default:
                    throw new Exception("SbQueueProvider get 'IMessageSerializer<T>' service error.");
            }

            messageQueue.Initializer(queueName);

            return messageQueue;
        }

        #endregion
    }
}
