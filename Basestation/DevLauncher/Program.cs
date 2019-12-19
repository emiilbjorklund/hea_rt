using Basestation.Common;
using Basestation.Common.Abstractions;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevLauncher
{
    class Program
    {
        public class Options
        {
            [Option('y', "yml", Required = true, HelpText = "Yml to set up the system")]
            public string Yml { get; set; }

            [Option('w', "windows", Required = false, HelpText = "System services will open in new windows")]
            public bool OpenWindows { get; set; }

            [Option('r', "release", Required = false, HelpText = "Looks for paths in Release folder ")]
            public bool IsRelease { get; set; }
        }

        static async Task Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>((o) => 
            {
                var ymlPath = SystemComponentPaths.GetYmlPath(o.Yml);
                var structure = new SystemStructure(ymlPath);

                foreach (var s in structure.Services)
                {
                    var servicePath = SystemComponentPaths.GetWorkDir(o.IsRelease);
                    StartService(servicePath, s, ymlPath, o.OpenWindows);
                }

            });
            
            //await Task.Delay(30000);
        }

        static void StartService(string path, Service service, string yml, bool openWindows)
        {
            var process = new Process();
            process.StartInfo.WorkingDirectory = path;
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"Basestation.Service.dll /port={service.Port} /id={service.Id} /yml={Path.GetFullPath(yml)}";
            if (openWindows)
            {
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.UseShellExecute = true;
            }

            process.Start();
        }
    }
}
