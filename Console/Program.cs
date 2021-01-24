using Core.Repositories;
using Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pipeline.MongoDB;
using PipelineTask.MongoDB;
using Services;
using System;
using System.Threading.Tasks;

namespace Console {
    class Program {
        static IConfiguration AppConfiguration { get; set; }

        static async Task Main(string[] args) {
            AppConfiguration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(AppConfiguration)
                .AddSingleton<IPipelineRepository, PipelineMongoDbRepository>()
                .AddSingleton<IPipelineTaskRepository, PipelineTaskMongoDbRepository>()
                .AddSingleton<IPipelineService, PipelineService>()
                .AddSingleton<IPipelineTaskService, PipelineTaskService>()
                .AddSingleton<IPipelineRunner, PipelineRunner>()
                .BuildServiceProvider();

            var pipelineService = serviceProvider.GetService<IPipelineService>();

            string pid = "";

            if (args.Length > 0 && args[0] == "-pid") {
                pid = args[1];
            } else {
                System.Console.WriteLine("Please, start program with pipeline id:\nPipelineConsole.exe -pid YOUR_PIPELINE_ID_HERE");
                Environment.Exit(0);
            }

            int pipelineRunTime = await pipelineService.CalculateAsync(pid);
            if (pipelineRunTime >= 0) {
                System.Console.WriteLine(pipelineRunTime + " seconds");
                Environment.Exit(0);
            } else {
                System.Console.WriteLine("Аn error occurred during the calculations");
                Environment.Exit(1);
            }
        }
    }
}
