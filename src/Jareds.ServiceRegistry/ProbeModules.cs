using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Jareds.ServiceRegistry
{
    /// <summary>
    /// 探测模块
    /// </summary>
    internal class ProbeModules
    {
        static List<Type> types = new List<Type>();
        public static List<Type> GetModules<T>(List<string> searchedLocations)
        {
            if (types != null && types.Count > 0)
            {
                return types;
            }
            if (searchedLocations == null)
            {
                searchedLocations = new List<string>();
            }
            List<Type> factoryTypes = new List<Type>();
            List<string> hitLocations = new List<string>();
            var assembies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var ass in assembies)
            {
                try
                {
                    if (FindInterface<T>(ass, factoryTypes))
                    {
                        hitLocations.Add(ass.Location);
                    }
                    searchedLocations.Add(ass.Location);
                }
                catch
                {
                }
            }
            types = new List<Type>();
            types.AddRange(factoryTypes);
            return types;
        }
        static void GetAllFiles(List<FileInfo> list, string searchPath = null)
        {
            if (list == null)
            {
                return;
            }
            string defaultDir = AppDomain.CurrentDomain.BaseDirectory.ToLower();
            if (Directory.Exists(Path.Combine(defaultDir, "bin")))
            {
                defaultDir = Path.Combine(defaultDir, "bin");
            }
            string path = searchPath ?? defaultDir;
            DirectoryInfo dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*.dll");
            if (files != null)
            {
                list.AddRange(files.Where(it => !ExcludeAssembly(it.Name)));
            }
            var dirs = dir.GetDirectories();
            if (dirs == null || dirs.Length <= 0)
            {
                return;
            }
            foreach (var item in dirs)
            {
                GetAllFiles(list, item.FullName);
            }
        }
        private static List<string> excludePrefixList = new List<string>
            {
                "Microsoft.",
                "System."
            };
        private static bool ExcludeAssembly(string filename)
        {
            bool hit = false;
            foreach (string prefix in excludePrefixList)
            {
                hit = filename.StartsWith(prefix);
                if (hit)
                {
                    break;
                }
            }
            return hit;
        }

        private static bool FindInterface<T>(Assembly ass, List<Type> factoryTypes)
        {
            Type[] assTypes = null;
            try
            {
                assTypes = ass.GetTypes();
            }
            catch (Exception ex)
            {
                return false;
            }
            int hit = 0;
            if (assTypes != null)
            {
                foreach (Type ct in assTypes)
                {
                    if (!ct.IsClass || ct.IsAbstract)//不是class，或者是抽象class
                    {
                        continue;
                    }
                    if (typeof(T).IsAssignableFrom(ct))
                    {
                        factoryTypes.Add(ct);
                        hit++;
                    }
                }
            }
            return hit > 0;
        }
    }
}
