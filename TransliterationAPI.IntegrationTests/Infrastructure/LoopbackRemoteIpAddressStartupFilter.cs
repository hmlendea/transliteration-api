using System;
using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace TransliterationAPI.IntegrationTests.Infrastructure
{
    internal sealed class LoopbackRemoteIpAddressStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            => application =>
            {
                application.Use((httpContext, nextMiddleware) =>
                {
                    httpContext.Connection.RemoteIpAddress = IPAddress.Loopback;

                    return nextMiddleware();
                });
                next(application);
            };
    }
}