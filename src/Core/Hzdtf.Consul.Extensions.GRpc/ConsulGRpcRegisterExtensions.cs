using Hzdtf.Consul.Extensions.Common;
using Hzdtf.Consul.Extensions.GRpc;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using NConsul;
using NConsul.AspNetCore;
using System;
using System.Linq;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ConsulGRpc注册扩展类
    /// @ 黄振东
    /// </summary>
    public static class ConsulGRpcRegisterExtensions
    {
        /// <summary>
        /// 添加GRpc注册Consul
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="configJsonFilePath">配置JSON文件路径。如果传入为null，则默认为Config/consulConfig.json</param>
        /// <returns>服务</returns>
        public static IServiceCollection AddGRpcRegisterConsul(this IServiceCollection services, string configJsonFilePath = "Config/consulConfig.json")
        {
            // 将consul配置文件配置到服务里
            var config = new ConfigurationBuilder().AddJsonFile(configJsonFilePath).Build();
            services.Configure<ConsulOptions>(config);

            // 添加一个能获取本服务地址的服务
            services.AddSingleton(serviceProvider =>
            {
                var server = serviceProvider.GetRequiredService<IServer>();
                return server.Features.Get<IServerAddressesFeature>();
            });

            // 从JSON文件里解析对象
            var consulConfig = configJsonFilePath.ToJsonObjectFromFile<ConsulOptions>();
            if (consulConfig == null)
            {
                throw new ArgumentNullException("json文件内容反序列化ConsulOptions对象为空");
            }
            if (string.IsNullOrWhiteSpace(consulConfig.ConsulAddress))
            {
                throw new ArgumentNullException("Consul地址不能为空");
            }
            if (string.IsNullOrWhiteSpace(consulConfig.ServiceName))
            {
                throw new ArgumentNullException("服务名不能为空");
            }

            // 生成Consul注册所需要的对象
            var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(consulConfig.ConsulAddress);
                x.Datacenter = consulConfig.Datacenter;
            });
            services.AddSingleton(consulClient);
            services.Configure(new Action<NConsulOptions>(op =>
            {
                op.Address = consulConfig.ConsulAddress;
            }));

            ConsulGRpcInner.ConsulBuilder = new NConsulBuilder(consulClient);

            return services;
        }
    }
}
