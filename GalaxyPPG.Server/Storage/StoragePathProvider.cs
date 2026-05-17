using System;
using System.Configuration;
using System.IO;
using System.Web.Hosting;

namespace GalaxyPPG.Server.Storage
{
    public static class StoragePathProvider
    {
        private const string StorageRootKey = "GalaxyPpgStorageRoot";
        private const string DefaultStorageRoot = "~/App_Data/GalaxyPPG";

        public static string GetStorageRoot()
        {
            string configuredPath = ConfigurationManager.AppSettings[StorageRootKey];
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                configuredPath = DefaultStorageRoot;
            }

            if (configuredPath.StartsWith("~/", StringComparison.Ordinal))
            {
                string mappedPath = HostingEnvironment.MapPath(configuredPath);
                if (!string.IsNullOrWhiteSpace(mappedPath))
                {
                    return mappedPath;
                }

                string relativePath = configuredPath.Substring(2).Replace('/', Path.DirectorySeparatorChar);
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            }

            if (Path.IsPathRooted(configuredPath))
            {
                return configuredPath;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuredPath);
        }
    }
}
