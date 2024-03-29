﻿using Consul;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.RemoteService.Provider;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Consul.Extensions.Common
{
    /// <summary>
    /// Consul服务提供者
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class ConsulServicesProvider : IServicesProvider
    {
        #region 属性与字段

        /// <summary>
        /// Consul客户端
        /// </summary>
        private readonly ConsulClient consulClient;

        #endregion

        #region 构造方法

        /// <summary>
        /// 构造方法
        /// </summary>
        public ConsulServicesProvider()
            : this("Config/consulConfig.json".ToJsonObjectFromFile<ConsulBasicOption>())
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="options">选项配置</param>
        public ConsulServicesProvider(IOptions<ConsulBasicOption> options)
            : this(options.Value)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="options">选项配置</param>
        public ConsulServicesProvider(ConsulBasicOption options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("选项配置不能为null");
            }
            if (string.IsNullOrWhiteSpace(options.ConsulAddress))
            {
                throw new ArgumentNullException("Consul地址不能为空");
            }

            consulClient = new ConsulClient(config =>
            {
                config.Address = new Uri(options.ConsulAddress);
                config.Datacenter = options.Datacenter;
            });
        }

        #endregion

        #region IServiceProvider 接口

        /// <summary>
        /// 异步根据服务名获取地址数组
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="tag">标签</param>
        /// <returns>地址数组任务</returns>
        public async Task<string[]> GetAddresses(string serviceName, string tag = null)
        {
            var queryResult = await consulClient.Health.Service(serviceName, tag, true);
            var addresses = new List<string>(queryResult.Response.Length);
            for (var i = 0; i < queryResult.Response.Length; i++)
            {
                var res = queryResult.Response[i];
                var add = $"{res.Service.Address}:{res.Service.Port}";
                // 去掉重复的地址
                if (addresses.Contains(add))
                {
                    continue;
                }

                addresses.Add(add);
            }

            Console.WriteLine($"{DateTime.Now.ToFullFixedDateTime()} 服务名:{serviceName},consol远程获取的地址列表:{addresses.ToJsonString()}");

            return addresses.ToArray();
        }

        #endregion

        #region IDisposable 接口

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (consulClient != null)
            {
                consulClient.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// 析构方法
        /// </summary>
        ~ConsulServicesProvider()
        {
            Dispose();
        }
    }
}
