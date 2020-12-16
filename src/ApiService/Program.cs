using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;


       
CreateHostBuilder(args).Build().Run();
    

static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddDataProtection()
                                .UseCustomCryptographicAlgorithms(
                                new ManagedAuthenticatedEncryptorConfiguration()
                                {
                                    EncryptionAlgorithmType = typeof(Aes),
                                    EncryptionAlgorithmKeySize = 256,
                                    ValidationAlgorithmType = typeof(HMACSHA256)
                                });
                        // IDataProtector
                        services.AddSingleton(c => {
                            var protectionProvider = c.GetRequiredService<IDataProtectionProvider>();
                            return protectionProvider.CreateProtector("myTestPrupose");
                        });
                     
                    });
                    webBuilder.Configure(app => {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("/protect/{value}", async context =>
                            {
                                var requestedValue = (string)context.Request.RouteValues["value"];
                                var protectionProvider = context.Request.HttpContext.RequestServices.GetRequiredService<IDataProtector>();
                                await context.Response.WriteAsync(protectionProvider.Protect(requestedValue));
                            });
                            endpoints.MapGet("/unprotect/{value}", async context =>
                            {
                                var requestedValue = (string)context.Request.RouteValues["value"];
                                var protectionProvider = context.Request.HttpContext.RequestServices.GetRequiredService<IDataProtector>();
                                await context.Response.WriteAsync(protectionProvider.Unprotect(requestedValue));
                            });
                        });
                    });
                });
