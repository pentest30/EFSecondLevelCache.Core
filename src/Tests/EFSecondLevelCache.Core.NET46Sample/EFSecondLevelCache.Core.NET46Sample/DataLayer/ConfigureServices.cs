﻿using System;
using CacheManager.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using EFSecondLevelCache.Core.Contracts;

namespace EFSecondLevelCache.Core.NET46Sample.DataLayer
{
    public static class ConfigureServices
    {
        private static readonly Lazy<IServiceProvider> _serviceProviderBuilder =
            new Lazy<IServiceProvider>(getServiceProvider, LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// A lazy loaded thread-safe singleton
        /// </summary>
        public static IServiceProvider Instance { get; } = _serviceProviderBuilder.Value;

        public static IEFCacheServiceProvider GetEFCacheServiceProvider()
        {
            return Instance.GetRequiredService<IEFCacheServiceProvider>();
        }

        private static IServiceProvider getServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<SampleContext>(ServiceLifetime.Scoped);

            services.AddEFSecondLevelCache();

            services.AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
            services.AddSingleton(typeof(ICacheManagerConfiguration),
                new CacheManager.Core.ConfigurationBuilder()
                        .WithJsonSerializer()
                        .WithMicrosoftMemoryCacheHandle(instanceName: "MemoryCache1")
                        .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(10))
                        .DisablePerformanceCounters()
                        .DisableStatistics()
                        .Build());

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}