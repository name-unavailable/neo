using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Neo.Wallets;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Neo.Network.Restful;


namespace Neo.Network.GraphQL
{
    public class GraphQLServer : IDisposable
    {
        public Wallet Wallet { get; set; }
        public long MaxGasInvoke { get; }

        private IWebHost host;
        private readonly NeoSystem system;

        public GraphQLServer(NeoSystem system, Wallet wallet = null, long maxGasInvoke = default)
        {
            this.system = system;
            this.Wallet = wallet;
            this.MaxGasInvoke = maxGasInvoke;
        }

        public void Dispose()
        {
            if (host != null)
            {
                host.Dispose();
                host = null;
            }
        }

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
                services.AddScoped<IRestService, RestService>(s => new RestService(system, Wallet, MaxGasInvoke));
                services.AddScoped<GraphSchema>();
                services.AddGraphQL().AddGraphTypes(ServiceLifetime.Scoped);

                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            })
            .Configure(app =>
            {
                app.UseGraphQL<GraphSchema>();
                app.UseGraphQLPlayground
                   (new GraphQLPlaygroundOptions());
                app.UseMvc();
            })
            .Build();

            host.Start();
        }
    }
}
