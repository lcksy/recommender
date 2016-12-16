using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;

namespace CQSS.Common.Util
{
    public static class AssemblyLocator
    {
        public static IEnumerable<Assembly> GetAssemblies(bool isWebApplication)
        {
            if (isWebApplication)
                return BuildManager.GetReferencedAssemblies().Cast<Assembly>();
            else
                return AppDomain.CurrentDomain.GetAssemblies();
        }

        public static IEnumerable<Assembly> GetBinFolderAssemblies(bool isWebApplication)
        {
            var binFolder = isWebApplication
                ? HttpRuntime.AppDomainAppPath + "bin\\"
                : AppDomain.CurrentDomain.BaseDirectory;

            var dllFiles = Directory.GetFiles(binFolder, "*.dll", SearchOption.TopDirectoryOnly).ToList();

            var assemblies = new List<Assembly>();
            foreach (string dllFile in dllFiles)
            {
                var assembly = Assembly.LoadFile(dllFile);
                assemblies.Add(assembly);
            }

            return assemblies;
        }
    }
}