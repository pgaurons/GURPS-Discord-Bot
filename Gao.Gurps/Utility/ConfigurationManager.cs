using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gao.Gurps.Utility
{
    public static class ConfigurationManager
    {
        public static IConfiguration Configuration { get; set; }

        static ConfigurationManager()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }
    }
}
