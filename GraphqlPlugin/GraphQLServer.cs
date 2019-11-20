using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using Neo;
using Neo.Network;
using Neo.Wallets;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace GraphqlPlugin
{
    public class GraphQLServer : QueryServer
    {
        public GraphQLServer(NeoSystem system, Wallet wallet = null, long maxGasInvoke = default) : base(system, wallet, maxGasInvoke) { }

        public void Start(IPAddress bindAddress, int port, string sslCert = null, string password = null, string[] trustedAuthorities = null)
        {
            host = new WebHostBuilder().UseKestrel(options => options.Listen(bindAddress, port, listenOptions =>
            {
                if (string.IsNullOrEmpty(sslCert)) return;
                listenOptions.UseHttps(sslCert, password, httpsConnectionAdapterOptions =>
                {
                    if (trustedAuthorities is null || trustedAuthorities.Length == 0)
                        return;
                    httpsConnectionAdapterOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    httpsConnectionAdapterOptions.ClientCertificateValidation = (cert, chain, err) =>
                    {
                        if (err != SslPolicyErrors.None)
                            return false;
                        X509Certificate2 authority = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
                        return trustedAuthorities.Contains(authority.Thumbprint);
                    };
                });
            }))
            .ConfigureServices(services =>
            {
                services.AddScoped<IDependencyResolver>
                           (s => new FuncDependencyResolver(s.GetRequiredService));
                services.AddScoped<IQueryService, QueryService>(s => new QueryService(NeoSystem, Wallet, MaxGasInvoke));
                services.AddScoped<RootSchema>();
                services.AddGraphQL().AddGraphTypes(ServiceLifetime.Scoped);
                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            })
            .Configure(app =>
            {
                app.UseGraphQL<RootSchema>();
                app.UseGraphQLPlayground
                   (new GraphQLPlaygroundOptions());
                app.UseMvc();
            })
            .Build();

            host.Start();
        }
    }
}
