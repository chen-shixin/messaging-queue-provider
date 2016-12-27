using Messaging.Queue.Provider.Domain.Enum;
using Messaging.Queue.Provider.Domain.Service.Interfaces.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Domain.Service.Factories
{
    public interface IMsmqProviderFactory
    {
        IMsmq<T> Create<T>(Serializer serializer, string queueName) where T : class;
    }
}
