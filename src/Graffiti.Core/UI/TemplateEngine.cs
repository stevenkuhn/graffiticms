using System.IO;
using System.Web;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;

namespace Graffiti.Core
{
    /// <summary>
    /// Summary description for TemplateEngine
    /// </summary>
    public static class TemplateEngine
    {

        private static bool _isInitialized = false;
        private static object lockedObject = new object();
        public static VelocityEngine engine = null;

        private static void Initialize()
        {
            if (!_isInitialized)
            {
                lock (lockedObject)
                {
                    if (!_isInitialized)
                    {
                        engine = new VelocityEngine();
                        
                        engine.SetProperty(RuntimeConstants.RUNTIME_LOG_LOGSYSTEM_CLASS, "NVelocity.Runtime.Log.NullLogSystem");
                        engine.SetProperty(RuntimeConstants.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl");
                        engine.SetProperty(RuntimeConstants.COUNTER_NAME, "count");
                        //engine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH,HttpContext.Current.Server.MapPath("~/files/themes/"));
                        engine.Init();

                        _isInitialized = true;
                    }
                }
            }
        }

        public static string Evaluate(string templatedata, IContext iContext)
        {
            Initialize();
            using (TextWriter tw = new StringWriter())
            {
                Evaluate(tw, templatedata, iContext);
                return tw.ToString();
            }
        }

        public static void Evaluate(TextWriter tw, string templatedata, IContext iContext)
        {
            Initialize();
            engine.Evaluate(iContext, tw, "Control.Evaluate", templatedata);
        }

    }
}