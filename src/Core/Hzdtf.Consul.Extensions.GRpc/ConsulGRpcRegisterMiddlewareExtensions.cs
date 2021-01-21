using Hzdtf.Consul.Extensions.Common;
using Hzdtf.Consul.Extensions.GRpc.Core.Services;
using Hzdtf.Utility.Utils;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NConsul;
using System;
using System.Linq;
using System.Text;
using Hzdtf.Consul.Extensions.GRpc;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// ConsulGRpc注册中间件扩展类
    /// @ 黄振东
    /// </summary>
    public static class ConsulGRpcRegisterMiddlewareExtensions
    {
        /// <summary>
        /// 使用GRpc注册Consul
        /// </summary>
        /// <param name="app">应用生成器</param>
        /// <returns>应用生成器</returns>
        public static IApplicationBuilder UseGRpcRegisterConsul(this IApplicationBuilder app)
        {
            // 获取consul配置对象
            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ConsulOptions>>().Value;

            // 获取本服务的地址，如果不为空，则直接取。否则取配置选项里的服务地址
            if (string.IsNullOrWhiteSpace(consulConfig.ServiceAddress))
            {
                consulConfig.ServiceAddress = NetworkUtil.FilterUrl(app.ApplicationServices.GetService<IServerAddressesFeature>().Addresses.FirstOrDefault());
                if (string.IsNullOrWhiteSpace(consulConfig.ServiceAddress))
                {
                    throw new ArgumentNullException("服务地址不能为空");
                }
            }

            // 注册到Consul
            var serviceUri = new Uri(consulConfig.ServiceAddress);
            var agentCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(consulConfig.ServiceCheck.DeregisterCriticalServiceAfter),
                Interval = TimeSpan.FromSeconds(consulConfig.ServiceCheck.Interval),
                GRPC = $"{serviceUri.Host}:{serviceUri.Port}",
                GRPCUseTLS = string.Compare(serviceUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) == 0,
                Timeout = TimeSpan.FromSeconds(consulConfig.ServiceCheck.Timeout),                
            };
            ConsulGRpcInner.ConsulBuilder.AddHealthCheck(agentCheck)
                         .RegisterService(consulConfig.ServiceName, serviceUri.Host, serviceUri.Port, consulConfig.Tags);

            return app;
        }

        /// <summary>
        /// 使用GRpc路由
        /// </summary>
        /// <param name="endpoints">终结点路由生成器</param>
        /// <returns>终结点路由生成器</returns>
        public static IEndpointRouteBuilder UseGRpcRoute(this IEndpointRouteBuilder endpoints)
        {
            // 映射健康检测路由
            endpoints.MapGrpcService<HealthCheckService>();

            return endpoints;
        }
    }
}
