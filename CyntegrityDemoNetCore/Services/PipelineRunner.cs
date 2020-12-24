using CyntegrityDemoNetCore.Hubs;
using CyntegrityDemoNetCore.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace CyntegrityDemoNetCore.Services {
    public class PipelineRunner {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<PipelineHub> _hub;
        public PipelineRunner(IConfiguration configuration, IHubContext<PipelineHub> hubContext) {
            _configuration = configuration;
            _hub = hubContext;
        } 
        public async System.Threading.Tasks.Task<int> RunAsync(Pipeline pipeline) {
            string cwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string relativeFilePath = _configuration["PipelineRunner:Path"];

            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo {
                FileName = cwd + relativeFilePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = "-pid " + pipeline.Id
            };
            var process = System.Diagnostics.Process.Start(processStartInfo);

            string line = string.Empty;
            while (!process.StandardOutput.EndOfStream) {
                line = process.StandardOutput.ReadLine();
            }

            _ = int.TryParse(line.Split(' ')[0], out int result);
            await _hub.Clients.All.SendAsync(pipeline.Id.ToString(), pipeline.Name);
            return result;
        }
    }
}
