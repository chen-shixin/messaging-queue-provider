# Message Queue Provider

Message.Queue.Provider is a simple implemetation for to peek, push and receive messaging.

## Providers

<a href="https://azure.microsoft.com/services/service-bus/" rel="NuGet">Service Bus</a>. </br>
<a href="https://msdn.microsoft.com/en-us/library/ms711472(v=vs.85).aspx" rel="NuGet">Microsoft Message Queuing</a>.

The service factories contains all providers:

```c#
  MsmqProviderFactory
  ServiceBusProviderFactory
```

## Getting Started

To get started with the Message.Queue.Provider, install <a href="https://www.nuget.org/packages/Messaging.Queue.Provider/" rel="NuGet">nuget package</a>.

Define the provider do you like use and install correlation package.

**Ms Message Queuing**: <a href="https://www.nuget.org/packages/Messaging.Queue.Provider.Msmq/" rel="NuGet">Msmq nuget package</a></br>
**Service Bus**: <a href="https://www.nuget.org/packages/Messaging.Queue.Provider.ServiceBus/" rel="NuGet">Service Bus nuget package</a> 

### Sample

```c#
// Dependency Injection (DI) using Simple Injector
var container = new Container();
container.RegisterSingleton<IServiceProvider>(container);

//If Msmq
container.Register<IMsmqProviderFactory, MsmqProviderFactory>();
 var msmqProviderFactory = container.GetInstance<IMsmqProviderFactory>();
 
//If Service Bus
container.Register<IServiceBusProviderFactory, ServiceBusProviderFactory>();
 var sbProviderFactory = container.GetInstance<IServiceBusProviderFactory>();
```

###### Push

```c#
var sampleMessage = new SampleMessage
{
  SampleMessageId = 1,
  Name = "Sample Message",
  Created = DateTime.UtcNow
};

//If do you like use Service Bus, alter next line for sbProviderFactory.Create...
using (var messageQueue = msmqProviderFactory.Create<SampleMessage>(Serializer.Json, "message_queue_sample"))
{
  await messageQueue.PushAsync(new QueueMessage<SampleMessage>(sampleMessage)).Wait();
  System.Console.WriteLine($"Push message successfully. Body: {queueMessage.Item}");
}
```

###### Receive

```c#
var isRunning = true;

//If do you like use Service Bus, alter next line for sbProviderFactory.Create...
using (var messageQueue = msmqProviderFactory.Create<SampleMessage>(Serializer.Json, "message_queue_sample"))
{
  while (isRunning)
  {
    try
    {
      var queueMessage = await messageQueue.ReceiveAsync(new TimeSpan(0, 0, 0, 10, 0)).Result;
  
      if (queueMessage != null)
      {
        System.Console.WriteLine($"Removed message from the queue. Body: {queueMessage.Item}");
      }
    }
    catch (TimeoutException ex)
    {
      System.Console.WriteLine($"{ex.Message}");
    }
    catch (Exception e)
    {
      System.Console.WriteLine($"Unexpected error receive message. Error: {ex.Message}");
    }
  }
}
```

###### .config

if using service bus provider

```xml
 <appSettings>
    <!-- Service Bus specific app setings for messaging connections -->
    <add key="sbConnectionString"
      value="Endpoint=sb://[your namespace].servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;
        SharedAccessKey=[your secret]"/>
  </appSettings>
```
