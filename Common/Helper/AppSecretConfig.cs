using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper
{

    public class AppSecretConfig
    {
        private static IConfiguration Configuration;
        public static string Audience_Secret_String => InitAudience_Secret();

        public AppSecretConfig(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private static string InitAudience_Secret()
        {
            var Audience_Secret = Configuration["Audience:Secret"];
            var Audience_Secret_File = Configuration["Audience:SecretFile"];
            var securityString = DifDBConnOfSecurity(Audience_Secret_File);
            if (!string.IsNullOrEmpty(Audience_Secret_File) && !string.IsNullOrEmpty(securityString))
            {
                return securityString;
            }
            else
            {
                return Audience_Secret;
            }

        }

        private static string DifDBConnOfSecurity(params string[] conn)
        {
            foreach (var item in conn)
            {
                try
                {
                    if (File.Exists(item))
                    {
                        return File.ReadAllText(item).Trim();
                    }
                }
                catch (System.Exception) { }
            }

            return conn[conn.Length - 1];
        }
    }
}
