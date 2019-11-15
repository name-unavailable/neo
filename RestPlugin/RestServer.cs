using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using Neo;
using Neo.Wallets;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Neo.Network;


namespace RestPlugin
{
    public class RestServer : QueryServer
    {
        public RestServer(NeoSystem system, Wallet wallet = null, long maxGasInvoke = default) : base(system, wallet, maxGasInvoke) { }

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
                services.AddSwaggerGen(option =>
                {
                    option.SwaggerDoc("v1", new Info
                    {
                        Title = "Neo Rest API",
                        Version = "v1"
                    });

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, "Restful.xml");
                    option.IncludeXmlComments(xmlPath);
                });

                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

                services.AddScoped<IQueryService, QueryService>(s => new QueryService(NeoSystem, Wallet, MaxGasInvoke));
            })
            .Configure(app =>
            {
                //app.UseHttpsRedirection();

                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Neo Rest API");
                    c.RoutePrefix = string.Empty;
                });

                app.UseMvc();
            })
            .Build();

            host.Start();
        }

    }
}
