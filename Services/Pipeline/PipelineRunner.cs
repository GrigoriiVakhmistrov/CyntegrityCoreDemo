using Core.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Services {
    public interface IPipelineRunner {
        public int Run(IPipeline pipeline);
    }
    public class PipelineRunner : IPipelineRunner {
        private readonly IConfiguration _configuration;
        public PipelineRunner(IConfiguration configuration) {
            _configuration = configuration;
        }
        public int Run(IPipeline pipeline) {
            int result = -1;
            if (pipeline == null)
                return result;

            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo {
                FileName = GetFilePath(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = "-pid " + pipeline.Id
            };
            System.Diagnostics.Process process;
            string line = string.Empty;
            try {
                process = System.Diagnostics.Process.Start(processStartInfo);
                while (!process.StandardOutput.EndOfStream) {
                    line = process.StandardOutput.ReadLine();
                }
            } catch {
                return result;
            }

            if (!int.TryParse(line.Split(" ")[0], out result))
                result = -1;

            return result;
        }
        string GetFilePath() {
            string cwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string relativeFilePath = _configuration["PipelineRunner:Path"];
            return cwd + relativeFilePath;
        }
    }
}
