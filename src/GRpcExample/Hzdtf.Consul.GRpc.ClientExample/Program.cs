using Grpc.Net.Client;
using Hzdtf.Consul.Extensions.Common;
using Hzdtf.Consul.GRpc.ServiceExample;
using Hzdtf.Utility.RemoteService.Builder;
using Hzdtf.Utility.RemoteService.Options;
using System;
using System.Threading;

namespace Hzdtf.Consul.GRpc.ClientExample
{
    class Program
    {
        static void Main(string[] args)
        {
            AppContextExtensions.SetHttp2UnencryptedSupport();

            Console.WriteLine("start grpc test...");

            var serviceProvider = new ConsulServicesProviderMemory();
            var serviceOptions = new UnityServicesOptionsCache();
            var unityServicesBuilder = new UnityServicesBuilder(serviceProvider, serviceOptions);

            for (var i = 0; i < 20; i++)
            {
                var url = unityServicesBuilder.BuilderAsync("GRpcServiceExampleA").Result;
                GRpcChannelUtil.CreateChannel(url, (channel, header) =>
                {
                    var client = new Greeter.GreeterClient(channel);
                    var res = client.SayHello(new HelloRequest()
                    {
                        Name = "张三" + i
                    });
                    Console.WriteLine($"第{i}次请求[{url}]:{res.ToJsonString()}");

                    Thread.Sleep(1000);
                });
            }

            Console.ReadLine();
        }
    }
}
