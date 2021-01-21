using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Hosting.Server;
using Hzdtf.Consul.Extensions.Common;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Consul服务注册扩展类
    /// @ 黄振东
    /// </summary>
    public static class ConsulRegisterExtensions
    {
        /// <summary>
        /// 添加服务注册Consul
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="configJsonFilePath">配置JSON文件路径。如果传入为null，则默认为Config/consulConfig.json</param>
        /// <returns>服务</returns>
        public static IServiceCollection AddRegisterConsul(this IServiceCollection services, string configJsonFilePath = "Config/consulConfig.json")
        {
            services.AddHealthChecks();

            // 将consul配置文件配置到服务里
            var config = new ConfigurationBuilder().AddJsonFile(configJsonFilePath).Build();
            services.Configure<ConsulOptions>(config);

            // 添加一个能获取本服务地址的服务
            services.AddSingleton(serviceProvider =>
            {
                var server = serviceProvider.GetRequiredService<IServer>();
                return server.Features.Get<IServerAddressesFeature>();
            });

            return services;
        }
    }
}
