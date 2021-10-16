using ReversePack.PluginCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ReversePack
{
    internal class PluginCollection
    {
        public List<IHeatFilter> HeatFilters { get; } = new List<IHeatFilter>();
        public List<IMapFunction> MapFunctions { get; } = new List<IMapFunction>();
    }

    internal class PluginLoader
    {
        private readonly DirectoryInfo PluginDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Plugins"));

        private List<Assembly> LoadAssemblies(DirectoryInfo directory, out List<(FileInfo, Exception)> failedToLoad)
        {
            var files = directory.GetFiles("*.dll");

            var assemblies = new List<Assembly>();
            failedToLoad = new List<(FileInfo, Exception)>();

            foreach (var file in files)
            {
                try
                {
                    assemblies.Add(Assembly.LoadFile(file.FullName));
                }
                catch (Exception e)
                {
                    failedToLoad.Add((file, e));
                }
            }
            return assemblies;
        }

        internal class FailureInfo
        {
            public List<(FileInfo, Exception)> FailedToLoadAssemblies { get; } = new List<(FileInfo, Exception)>();
            public List<(Assembly, Exception)> FailedToGetTypesFromAssembly { get; } = new List<(Assembly, Exception)>();
            public List<(Type, Exception)> FailedToActivateType { get; set; } = new List<(Type, Exception)>();

            public bool AnyFailures => FailedToLoadAssemblies.Count > 0 || FailedToGetTypesFromAssembly.Count > 0 || FailedToActivateType.Count > 0;
        }

        public PluginCollection LoadPlugins(out FailureInfo failures)
        {
            failures = new FailureInfo();

            var plugins = new PluginCollection();

            if (!PluginDirectory.Exists)
            {
                return plugins;
            }

            List<Assembly> assemblies = LoadAssemblies(PluginDirectory, out List<(FileInfo, Exception)> failed);
            failures.FailedToLoadAssemblies.AddRange(failed);

            List<Type> types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    Type[] assemblyTypes = assembly.GetTypes();
                    types.AddRange(assemblyTypes);
                }
                catch (Exception e)
                {
                    failures.FailedToGetTypesFromAssembly.Add((assembly, e));
                }
            }

            foreach (var type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (typeof(IHeatFilter).IsAssignableFrom(type))
                {
                    try
                    {
                        plugins.HeatFilters.Add((IHeatFilter)Activator.CreateInstance(type));
                    }
                    catch (Exception e)
                    {
                        failures.FailedToActivateType.Add((type, e));
                    }

                }
                else if (typeof(IMapFunction).IsAssignableFrom(type))
                {
                    try
                    {
                        plugins.MapFunctions.Add((IMapFunction)Activator.CreateInstance(type));
                    }
                    catch (Exception e)
                    {
                        failures.FailedToActivateType.Add((type, e));
                    }
                }
            }

            return plugins;
        }
    }
}
