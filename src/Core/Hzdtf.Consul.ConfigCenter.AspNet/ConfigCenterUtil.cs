using Hzdtf.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Consul.ConfigCenter.AspNet
{
    /// <summary>
    /// 配置中心辅助类
    /// @ 黄振东
    /// </summary>
    public static class ConfigCenterUtil
    {
        /// <summary>
        /// 获取键路径，以/分隔
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="serviceName">服务名，如果为空，则默认取UtilTool.AppServiceName</param>
        /// <returns>键路径</returns>
        public static string GetKeyPath(string key, string serviceName = null)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                serviceName = App.AppServiceName;
            }

            return string.IsNullOrWhiteSpace(serviceName) ? key : $"{serviceName}/{key}";
        }
    }
}
