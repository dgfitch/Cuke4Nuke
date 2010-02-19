using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cuke4Nuke.Core
{
    public class Loader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        readonly IEnumerable<string> _assemblyPaths;
        readonly ObjectFactory _objectFactory;

        public Loader(IEnumerable<string> assemblyPaths, ObjectFactory objectFactory)
        {
            _assemblyPaths = assemblyPaths;
            _objectFactory = objectFactory;
        }

        public virtual Repository Load()
        {
            var repository = new Repository();

            foreach (var assemblyPath in _assemblyPaths)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var configPath = assemblyPath + ".config";
                if (System.IO.File.Exists(configPath)) {
                    System.Diagnostics.Debug.WriteLine("cuke4nuke loading " + configPath);
                    LoadConfig(configPath);
                } else {
                    System.Diagnostics.Debug.WriteLine("cuke4nuke could not find " + configPath);
                }
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var method in type.GetMethods(StepDefinition.MethodFlags))
                    {
                        if (StepDefinition.IsValidMethod(method))
                        {
                            repository.StepDefinitions.Add(new StepDefinition(method));
                            _objectFactory.AddClass(method.ReflectedType);
                        }
                        if (BeforeHook.IsValidMethod(method))
                        {
                            repository.BeforeHooks.Add(new BeforeHook(method));
                            _objectFactory.AddClass(method.ReflectedType);
                        }
                        if (AfterHook.IsValidMethod(method))
                        {
                            repository.AfterHooks.Add(new AfterHook(method));
                            _objectFactory.AddClass(method.ReflectedType);
                        }
                    }
                }
            }

            return repository;
        }

        private void LoadConfig(string configPath) {
            // Somehow merge the appsettings from that file into the configuration manager
            // This is partially taken from the comments on http://www.codeproject.com/KB/dotnet/dllappconfig.aspx
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configPath);
            FieldInfo[] fiStateValues = null;
            Type tInitState = typeof(System.Configuration.ConfigurationManager).GetNestedType("InitState",BindingFlags.NonPublic);
            if (null != tInitState)
                fiStateValues = tInitState.GetFields();

            FieldInfo fiInit = typeof(System.Configuration.ConfigurationManager).GetField("s_initState",BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo fiSystem = typeof(System.Configuration.ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);
            if (fiInit != null && fiSystem != null && null != fiStateValues)
            {
                fiInit.SetValue(null, fiStateValues[1].GetValue(null));
                fiSystem.SetValue(null, null);
            }
        }
    }
}