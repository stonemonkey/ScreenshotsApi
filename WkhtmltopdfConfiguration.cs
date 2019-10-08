using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Screenshots
{
    public static class WkhtmltopdfConfiguration
    {
        public static string Path { get; set; }

        public static IServiceCollection AddWkhtmltopdf(this IServiceCollection services, string wkhtmltopdfRelativePath = "Wkhtmltopdf")
        {
            Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, wkhtmltopdfRelativePath);

            if (!Directory.Exists(Path))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    throw new Exception("Folder containing wkhtmltopdf.exe not found, searched for " + Path);
                }

                throw new Exception("Folder containing wkhtmltopdf not found, searched for " + Path);
            }
            
            return services;
        }
    }
}
