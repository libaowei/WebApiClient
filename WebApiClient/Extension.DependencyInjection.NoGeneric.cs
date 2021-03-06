﻿# if NETCOREAPP2_1

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace WebApiClient
{
    /// <summary>
    /// 提供项目相关扩展
    /// </summary>
    public static partial class Extension
    {
        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi(this IServiceCollection services, Type httpApiType)
        {
            return services.AddHttpApi(httpApiType, (o, s) => { });
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions> configureOptions)
        {
            return services.AddHttpApi(httpApiType, (o, s) => configureOptions(o));
        }

        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType"></param>
        /// <param name="configureOptions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHttpClientBuilder AddHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions, IServiceProvider> configureOptions)
        {
            var type = typeof(HttpApiBuilder<>).MakeGenericType(httpApiType);
            var builder = Lambda.CreateCtorFunc<IServiceCollection, IHttpApiBuilder>(type)(services);
            return builder.AddHttpApi(configureOptions);
        }

        /// <summary>
        /// 配置HttpApi
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, Action<HttpApiOptions> configureOptions)
        {
            var type = typeof(HttpApiBuilder<>).MakeGenericType(httpApiType);
            var builder = Lambda.CreateCtorFunc<IServiceCollection, IHttpApiBuilder>(type)(services);
            return builder.ConfigureHttpApi(configureOptions);
        }

        /// <summary>
        /// 配置HttpApi
        /// </summary>
        /// <param name="services"></param>
        /// <param name="httpApiType"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureHttpApi(this IServiceCollection services, Type httpApiType, IConfiguration configureOptions)
        {
            var type = typeof(HttpApiBuilder<>).MakeGenericType(httpApiType);
            var builder = Lambda.CreateCtorFunc<IServiceCollection, IHttpApiBuilder>(type)(services);
            return builder.ConfigureHttpApi(configureOptions);
        }

        /// <summary>
        /// 定义httpApi的Builder的行为
        /// </summary>
        private interface IHttpApiBuilder
        {
            /// <summary>
            /// 添加HttpApi代理类到服务
            /// </summary>
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            IHttpClientBuilder AddHttpApi(Action<HttpApiOptions, IServiceProvider> configureOptions);

            /// <summary>
            /// 配置HttpApi
            /// </summary>
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            IServiceCollection ConfigureHttpApi(Action<HttpApiOptions> configureOptions);

            /// <summary>
            /// 配置HttpApi
            /// </summary>
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            IServiceCollection ConfigureHttpApi(IConfiguration configureOptions);
        }


        /// <summary>
        /// httpApi的Builder
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        private class HttpApiBuilder<THttpApi> : IHttpApiBuilder where THttpApi : class, IHttpApi
        {
            private readonly IServiceCollection services;

            /// <summary>
            /// httpApi的Builder
            /// </summary>
            /// <param name="services"></param>
            public HttpApiBuilder(IServiceCollection services)
            {
                this.services = services;
            }

            /// <summary>
            /// 添加HttpApi代理类到服务
            /// </summary>
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            public IHttpClientBuilder AddHttpApi(Action<HttpApiOptions, IServiceProvider> configureOptions)
            {
                return this.services.AddHttpApi<THttpApi>((o, s) => configureOptions(o, s));
            }

            /// <summary>
            /// 配置HttpApi
            /// </summary> 
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            public IServiceCollection ConfigureHttpApi(Action<HttpApiOptions> configureOptions)
            {
                return this.services.ConfigureHttpApi<THttpApi>(o => configureOptions(o));
            }

            /// <summary>
            /// 配置HttpApi
            /// </summary>
            /// <param name="configureOptions"></param>
            /// <returns></returns>
            public IServiceCollection ConfigureHttpApi(IConfiguration configureOptions)
            {
                return this.services.ConfigureHttpApi<THttpApi>(configureOptions);
            }
        }
    }
}

#endif