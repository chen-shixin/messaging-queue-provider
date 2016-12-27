using Messaging.Queue.Provider.Domain.Enum;
using Messaging.Queue.Provider.Domain.Service;
using Messaging.Queue.Provider.Domain.Service.Factories;
using Messaging.Queue.Provider.Infra.Msmq.Factories;
using Messaging.Queue.Provider.Infra.ServiceBus.Factories;
using Messaging.Queue.Provider.Samples.Console.Message;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Samples.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container();

            try
            {
                #region DI

                container.RegisterSingleton<IServiceProvider>(container);
                container.Register<IMsmqProviderFactory, MsmqProviderFactory>();
                container.Register<IServiceBusProviderFactory, ServiceBusProviderFactory>();

                #endregion

                var msmqProviderFactory = container.GetInstance<IMsmqProviderFactory>();

                MsmqPushMessage(msmqProviderFactory);

                MsmqPurgeMessage(msmqProviderFactory);

                MsmqPushMessage(msmqProviderFactory);

                MsmqReceiveMessage(msmqProviderFactory);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Unexpected error: " + ex.Message);
                System.Console.ReadLine();
            }

            System.Console.ReadLine();
        }

        #region Private Methods

        /// <summary>
        /// Push msmq message
        /// </summary>
        /// <param name="msmqProviderFactory"></param>
        private static void MsmqPushMessage(IMsmqProviderFactory msmqProviderFactory)
        {
            using (var messageQueue = msmqProviderFactory.Create<SampleMessage>(Serializer.Json, "message_queue_sample_console"))
            {
                for (int i = 0; i < 5; i++)
                {
                    var sampleMessage = new SampleMessage
                    {
                        SampleMessageId = i,
                        Name = "Sample Message",
                        Created = DateTime.UtcNow
                    };

                    messageQueue.PushAsync(new QueueMessage<SampleMessage>(sampleMessage)).Wait();
                }
            }
        }

        /// <summary>
        /// Receive msmq message
        /// </summary>
        private static void MsmqReceiveMessage(IMsmqProviderFactory msmqProviderFactory)
        {
            var isRunning = true;

            using (var messageQueue = msmqProviderFactory.Create<SampleMessage>(Serializer.Json, "message_queue_sample_console"))
            {
                while (isRunning)
                {
                    QueueMessage<SampleMessage> queueMessage = null;

                    try
                    {
                        queueMessage = messageQueue.ReceiveAsync(new TimeSpan(0, 0, 0, 10, 0)).Result;

                        if (queueMessage != null)
                        {
                            System.Console.WriteLine(string.Format("Removed message from the queue. Body: {0}", queueMessage.Item));
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Unexpected error: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Purge msmq message
        /// </summary>
        private static void MsmqPurgeMessage(IMsmqProviderFactory msmqProviderFactory)
        {
            using (var messageQueue = msmqProviderFactory.Create<SampleMessage>(Serializer.Json, "message_queue_sample_console"))
            {
                messageQueue.PurgeAsync().Wait();
            }
        }

        #endregion
    }
}
