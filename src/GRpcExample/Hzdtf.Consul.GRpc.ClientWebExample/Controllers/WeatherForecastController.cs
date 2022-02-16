using Grpc.Net.Client;
using Hzdtf.Consul.GRpc.ServiceExample;
using Hzdtf.Utility.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Hzdtf.Consul.GRpc.ServiceExample.Greeter;

namespace Hzdtf.Consul.GRpc.ClientWebExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IList<KeyValueInfo<string, string>> Get()
        {
            var list = new List<KeyValueInfo<string, string>>();
            var req = new HelloRequest();
            for (var i = 0; i < 100; i++)
            {
                var kv = new KeyValueInfo<string, string>();
                req.Name = "黄华英" + i;
                try
                {
                    GRpcChannelUtil.GetGRpcClientFormStrategy<GreeterClient>("GRpcServiceExampleA", (channel) =>
                    {
                        kv.Key = channel.Target;
                        return new GreeterClient(channel);
                    }, (client, header) =>
                    {
                        var res = client.SayHello(req);
                        kv.Value = res.Message;
                    });
                }
                catch (Exception ex)
                {
                    kv.Value = ex.Message;
                }
                list.Add(kv);

                Console.WriteLine($"{DateTime.Now.ToFullFixedDateTime()} 请求后结果:" + kv.ToJsonString());

                Thread.Sleep(1000);
            }

            return list;
        }
    }
}
