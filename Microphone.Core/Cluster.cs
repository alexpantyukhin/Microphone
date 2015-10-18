﻿using System;
using System.Linq;
using System.Text;
using Consul;

namespace Microphone.Core
{
    public static class Cluster
    {
        private static string ServiceName;
        private static string ServiceId;

        public static string GetConfig()
        {
            var client = new Client();
            var key = "ServiceConfig:" + ServiceName;
            var response = client.KV.Get(key);
            var res = Encoding.UTF8.GetString(response.Response.Value);
            return res;
        }

        public static ServiceInformation[] FindService(string name)
        {
            Logger.Information("{ServiceName} lookup {OtherServiceName}", ServiceName, name);
            var client = new Client();
            var others = client.Health.Service(name, null, true);

            return
                others.Response.Select(other => new ServiceInformation(other.Service.Address, other.Service.Port))
                    .ToArray();
        }

        public static void RegisterService(string serviceName, string serviceId, string version, Uri uri)
        {
            ServiceName = serviceName;
            ServiceId = serviceId;
            var client = new Client();
            client.Agent.ServiceRegister(new AgentServiceRegistration
            {
                Address = uri.Host,
                ID = serviceId,
                Name = serviceName,
                Port = uri.Port,
                Tags = new[] {version},
                Check = new AgentServiceCheck
                {
                    HTTP = uri + "status",
                    Interval = TimeSpan.FromSeconds(1),
                    TTL = TimeSpan.Zero,
                    Timeout = TimeSpan.Zero
                }
            });
        }
    }
}