using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevLauncher
{
    public static class SystemComponentPaths
    {
        public static string GetYmlPath(string ymlfile)
        {
            //Project root
            if (File.Exists(ymlfile))
                return ymlfile;

            //Navigate from ProjectRoot/Basestation.DevLauncher to ProjectRoot/Deployment
            var dotnetrunPath = Path.Combine("..", "Deployment", ymlfile);
            if (File.Exists(dotnetrunPath))
                return dotnetrunPath;

            //Navigate from ProjectRoot/Basestation.DevLauncher/bin/Debug||Release/netcoreapp3.0 to ProjectRoot/Deployment
            var vsstudioPath = Path.Combine("..", "..", "..", "..", "Deployment", ymlfile);
            if (File.Exists(vsstudioPath))
                return vsstudioPath;

            throw new Exception($"The file {ymlfile} could not be found");
        }

        public static string GetWorkDir(bool isRelease)
        {


            var dotnetrunPath = "";
            if (isRelease)
                dotnetrunPath = Path.Combine("..", "Basestation.Service", "bin", "Release", "netcoreapp3.0");
            else
                dotnetrunPath = Path.Combine("..", "Basestation.Service", "bin", "Debug", "netcoreapp3.0");
            if (Directory.Exists(dotnetrunPath))
                return dotnetrunPath;

            var vsstudioPath = "";
            if (isRelease)
                vsstudioPath = Path.Combine("..", "..", "..", "..", "Basestation.Service", "bin", "Release", "netcoreapp3.0");
            else
                vsstudioPath = Path.Combine("..", "..", "..", "..", "Basestation.Service", "bin", "Debug", "netcoreapp3.0");
            if (Directory.Exists(vsstudioPath))
                return vsstudioPath;

            var publishedPath = Path.Combine("..", "Basestation.Service");
            if (Directory.Exists(publishedPath))
                return publishedPath;

            throw new Exception($"Could not locate the executable dll file");
        }
    }
}
