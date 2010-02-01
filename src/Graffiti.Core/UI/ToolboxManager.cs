using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;

namespace Graffiti.Core
{
    public static class ToolboxManager
    {
        private static Dictionary<string, ConstructorInfo> requestOnly = new Dictionary<string, ConstructorInfo>();
        private static ApplicationToolboxContext appContext = new ApplicationToolboxContext();
        private static bool _isLoaded = false;
        private static readonly object lockedOnly = new object();

        public static ApplicationToolboxContext GetApplicationToolbox()
        {
            Load();
            return appContext;
        }

        //public static RequestToolboxContext GetRequestToolbox()
        //{
        //    Load();
        //    RequestToolboxContext rtc = new RequestToolboxContext();
        //    foreach (string key in requestOnly.Keys)
        //    {
        //        rtc.Put(key, requestOnly[key].Invoke(null));
        //    }
        //    return rtc;
        //}

        private static void Load()
        {
            if (!_isLoaded)
            {
                lock (lockedOnly)
                {
                    if (!_isLoaded)
                    {
                        string[] assemblies = Directory.GetFileSystemEntries(HttpRuntime.BinDirectory, "*.dll");

                        for (int i = 0; i < assemblies.Length; i++)
                        {
                            try
                            {
                                Assembly asm = Assembly.LoadFrom(assemblies[i]);
                                foreach (Type type in asm.GetTypes())
                                {
                                    try
                                    {
                                        ProcessType(type);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Warn("Chalk Extension",
                                                 "Failed to load type {0}. Reason: {1}", type.FullName, ex.Message);
                                    }
                                }
                            }

                            catch(ReflectionTypeLoadException rtle)
                            {
                                if (assemblies[i].IndexOf("DataBuddy") == -1 && assemblies[i].IndexOf("RssToolkit") == -1)
                                Log.Warn("Chalk Extension", "Failed to load assembly {0}. Reason: {1}",
                                         assemblies[i], rtle.Message);
                            }

                            catch (Exception exAssembly)
                            {
                                Log.Warn("Chalk Extension", "Failed to load assembly {0}. Reason: {1}",assemblies[i], exAssembly.Message);
                            }
                        }
                        _isLoaded = true;
                    }
                }
            }
        }

        private static void ProcessType(Type type)
        {
            if(type.IsClass)
            {
                object[] attributes = type.GetCustomAttributes(typeof(ChalkAttribute), false);
                if(attributes != null && attributes.Length > 0)
                {
                    ChalkAttribute tba = attributes[0] as ChalkAttribute;
                    if(tba != null)
                    {
                            appContext.Put(tba.Key, Activator.CreateInstance(type));
                    }
                }
            }
        }
    }
}