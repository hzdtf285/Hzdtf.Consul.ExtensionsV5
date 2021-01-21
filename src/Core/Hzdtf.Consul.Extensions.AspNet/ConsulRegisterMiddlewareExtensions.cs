using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Hzdtf.Consul.Extensions.Common;
using System.Linq;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Consul服务注册中间件扩展类
    /// @ 黄振东
    /// </summary>
    public static class ConsulRegisterMiddlewareExtensions
    {
        /// <summary>
        /// 使用服务注册Consul
        /// </summary>
        /// <param name="app">应用生成器</param>
        /// <returns>应用生成器</returns>
        public static IApplicationBuilder UseRegisterConsul(this IApplicationBuilder app)
        {
            // 获取consul配置对象
            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ConsulOptions>>().Value;

            // 使用健康检测服务
            app.UseHealthChecks(consulConfig.ServiceCheck.HealthCheck);

            // 创建consul客户端对象
            var consulClient = ConsulRegisterUtil.CreateConsulClientRegister(consulConfig, 
                () => app.ApplicationServices.GetService<IServerAddressesFeature>().Addresses.FirstOrDefault());

            return app;
        }
    }
}
