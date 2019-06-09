using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace dynamodb_test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dynamoDbConfig = Configuration.GetSection("DynamoDb");
            var runLocalDynamoDb = dynamoDbConfig.GetValue<bool>("LocalMode");

            // Quick n Dirty Fix for AWS Creds:
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "xxx");
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "yyy");
            Environment.SetEnvironmentVariable("AWS_REGION", "us-east-1");

            // Long Term Fix for AWS Creds:
            // https://github.com/aws/aws-sdk-net/issues/499
            // Specify location and setup AWS creds file in configuration files
            // Then set file accordingly
            // services.AddDefaultAWSOptions(Configuration.GetAWSOptions());


            if (runLocalDynamoDb)
            {
                services.AddSingleton<IAmazonDynamoDB>(sp =>
                {
                    var clientConfig = new AmazonDynamoDBConfig()
                    {
                        ServiceURL = dynamoDbConfig.GetValue<string>("LocalServiceUrl")
                    };

                    return new AmazonDynamoDBClient(clientConfig);
                });
            }
            else
            {
                services.AddAWSService<IAmazonDynamoDB>();
            }

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
