using AutoMapper;
using InventoryAPI.Configuration;
using InventoryAPI.OperationFilters;
using InventoryAPI.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace InventoryAPI
{
    public class Startup
    {
        private const string SwaggerVersion = "v1";
        private const string SwaggerTitle = "Inventory API";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("Version: " + System.Environment.Version + " from: " + System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());

            services.AddOptions(); // TODO: Check if needed

            services.Configure<MongoConnectionConfiguration>(Configuration.GetSection("MongoConnection"));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SwaggerVersion,
                    new Info
                    {
                        Title = SwaggerTitle,
                        Version = SwaggerVersion,
                        Description = "API for the Home Inventory Project."
                    });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.DescribeAllEnumsAsStrings();
                c.DescribeAllParametersInCamelCase();

                c.OperationFilter<GeneralResponsesOperationFilter>();
                c.OperationFilter<GetResponsesOperationFilter>();
                c.OperationFilter<PostResponsesOperationFilter>();
                c.OperationFilter<PutResponsesOperationFilter>();
                c.OperationFilter<DeleteResponsesOperationFilter>();
            });

            services.AddSingleton<IItemsRepository, ItemsRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{SwaggerVersion}/swagger.json", $"{SwaggerTitle} {SwaggerVersion}");
            });

            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
