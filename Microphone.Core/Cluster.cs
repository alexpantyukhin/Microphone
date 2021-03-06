﻿using System;
using System.Threading.Tasks;
using Microphone.Core.ClusterProviders;

namespace Microphone.Core
{
    public static class Cluster
    {
        private static IClusterProvider _clusterProvider;
        private static IFrameworkProvider _frameworkProvider;

        public static Task<ServiceInformation[]> FindServiceInstancesAsync(string name)
        {
            return _clusterProvider.FindServiceInstancesAsync(name);
        }

        public static Task<ServiceInformation> FindServiceInstanceAsync(string name)
        {
            return _clusterProvider.FindServiceInstanceAsync(name);
        }

        public static void BootstrapClient(IClusterProvider clusterProvider)
        {
            _clusterProvider = clusterProvider;
            _clusterProvider.BootstrapClientAsync().Wait();
        }

        public static Task KVPutAsync(string key, object value)
        {
            return _clusterProvider.KVPutAsync(key, value);
        }

        public static Task<T> KVGetAsync<T>(string key)
        {
            return _clusterProvider.KVGetAsync<T>(key);
        }

        public static void Bootstrap(IFrameworkProvider frameworkProvider, IClusterProvider clusterProvider, string serviceName, string version)
        {
            _frameworkProvider = frameworkProvider;
            var uri = _frameworkProvider.Start(serviceName, version);
            var serviceId = serviceName + Guid.NewGuid();
            _clusterProvider = clusterProvider;
            _clusterProvider.RegisterServiceAsync(serviceName, serviceId, version, uri).Wait();
        }          
    }
}