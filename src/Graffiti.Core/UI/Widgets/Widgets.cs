using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using DataBuddy;

namespace Graffiti.Core
{
    /// <summary>
    /// Widgets are stored in the Graffit ObjectStore. The Widgets class is responsible for convering ObjectStore 
    /// instances into an instance of Widget, reading widgets from assemblies, and managing their state.
    /// </summary>
    public static class Widgets
    {
        private static readonly string cacheKey = "Graffit.Core.Widgets.Cache";

        public static void Reset()
        {
            ZCache.RemoveCache(cacheKey);
        }

        /// <summary>
        /// Deletes a widget.
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(string id)
        {
            Query q = ObjectStore.CreateQuery();
            q.AndWhere(ObjectStore.Columns.Name, id);
            q.AndWhere(ObjectStore.Columns.ContentType, "xml/widget");
            ObjectStoreCollection osc = new ObjectStoreCollection();
            osc.LoadAndCloseReader(q.ExecuteReader());
            if (osc.Count > 1)
                throw new Exception("More than one item matched id/name ");
            else if (osc.Count > 0)
                ObjectStore.Destroy(osc[0].Id);

            
            Reset();
        }

        /// <summary>
        /// Gets all the widgets from the ObjectStore (match ContentType = "xml/widget"
        /// </summary>
        /// <returns></returns>
        public static List<Widget> FetchAll()
        {

            List<Widget> the_Widgets = ZCache.Get<List<Widget>>(cacheKey);
            if (the_Widgets == null)
            {
                ObjectStoreCollection osc = new ObjectStoreCollection();
                Query oquery = ObjectStore.CreateQuery();
                oquery.AndWhere(ObjectStore.Columns.ContentType, "xml/widget");
                osc.LoadAndCloseReader(oquery.ExecuteReader());

                the_Widgets = new List<Widget>(osc.Count);
                foreach (ObjectStore os in osc)
                {
                    try
                    {
                        Widget widget = ObjectManager.ConvertToObject(os.Data, Type.GetType(os.Type)) as Widget;

                        if (widget != null)
                            the_Widgets.Add(widget);
                        else
                            Log.Warn("Widgets",
                                         "The widget of type {0} (Widget Id:{1}, ObjetStore id: {2}) could not be loaded. Please check with the widget developer for help",
                                         os.Type, os.Name, os.Id);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Widget",
                                      "An exception was raised invoking the following widget type {0}. Exception: {1} Details: {2}",
                                      os.Type, ex.Message, ex.StackTrace);
                    }
                }
                
                ZCache.InsertCache(cacheKey,the_Widgets,120);
            }
            return the_Widgets;
        }


        /// <summary>
        /// Returns all Widgets for a given location. They are also sorted by O
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static List<Widget> FetchByLocation(WidgetLocation location)
        {
            List<Widget> filtered_widgets = new List<Widget>();
            foreach(Widget widget in FetchAll())
                if(widget.Location == location)
                    filtered_widgets.Add(widget);

                filtered_widgets.Sort(
                                    delegate(Widget wi1, Widget wi2)
                                        {
                                            return Comparer<int>.Default.Compare(wi1.Order,wi2.Order);
                                        });

            return filtered_widgets;
        }


        /// <summary>
        /// Stores the results of a re-order in the the ObjectStore
        /// </summary>
        /// <param name="id">The idea of the element (lbar, rbar,qbar). This value will be used as the delimiter in the list paramater</param>
        /// <param name="list">A serialized delimited list of widget ids. It should use the pattern &id[]=Guid&id[]Guid</param>
        public static void ReOrder(string id, string list)
        {
            string[] saList = list.Split(new string[] {"&"}, StringSplitOptions.RemoveEmptyEntries);

            WidgetLocation wl = WidgetLocation.Right;
            if (id == "qbar") wl = WidgetLocation.Queue;
            if (id == "lbar") wl = WidgetLocation.Left;

            List<Widget> the_Widgets = FetchAll();
            int i = 0;

            foreach (string wid in saList)
            {
                foreach (Widget widget in the_Widgets)
                {
                    string gid = widget.Id.ToString();

                    if (gid == wid)
                    {
                        widget.Location = wl;
                        widget.Order = i;
                        i++;
                        Save(widget,false);
                    }
                }
            }

            Reset();
        }

        /// <summary>
        /// Fetches a single Widget
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Widget Fetch(Guid id)
        {
            List<Widget> the_Widgets = FetchAll();
            foreach(Widget widget in the_Widgets)
            {
                if (widget.Id == id)
                    return widget;
            }

            Log.Warn("Widget", "The widget with the id of {0} could not be found.", id);

            return null;
        }

        public static Widget FetchByTitle(string title)
        {
            List<Widget> the_Widgets = FetchAll();
            foreach (Widget widget in the_Widgets) {
                if (widget.Title == title)
                    return widget;
            }

            Log.Warn("Widget", "The widget with the title of {0} could not be found.", title);

            return null;            
        }

        /// <summary>
        /// Creates an empty Widget for the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Widget Create(Type type)
        {
            Widget widget = Activator.CreateInstance(type) as Widget;
            widget.Id = Guid.NewGuid();
            widget.Order = Widgets.FetchByLocation(WidgetLocation.Queue).Count + 1;
				widget.SetValues(HttpContext.Current, widget.GetDefaults());
            ObjectStore os = new ObjectStore();
            os.ContentType = "xml/widget";

            string the_Type = type.AssemblyQualifiedName;
            os.Type = the_Type.Substring(0, the_Type.IndexOf(", Version="));
            os.Data = ObjectManager.ConvertToString(widget);
            os.Name = widget.Id.ToString();
            os.UniqueId = widget.Id;
            os.Save();

            Reset();

            return widget;
        }

        private static void Save(Widget widget, bool resetCache)
        {
            ObjectStore os = ObjectStore.FetchByColumn(ObjectStore.Columns.Name, widget.Id.ToString());
            os.Data = ObjectManager.ConvertToString(widget);
            os.Save();

            if(resetCache)
                Reset();
        }

        /// <summary>
        /// Saves the widget. It must first be convered to the ObjectStore instance.
        /// </summary>
        /// <param name="widget"></param>
        public static void Save(Widget widget)
        {
            Save(widget,true);
        }

        private static bool _isLoaded = false;
        private static readonly object lockedOnly = new object();
        private static List<WidgetDescription> _widgets = new List<WidgetDescription>();

        /// <summary>
        /// Returns all of the known/avaialble widgets based on their WidgetInfo
        /// </summary>
        /// <returns></returns>
        public static List<WidgetDescription> GetAvailableWidgets()
        {
            Load();
            return _widgets;
        }

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
                                    if (type.IsClass && type.IsSubclassOf(typeof(Widget)))
                                    {
                                        try
                                        {
                                            ProcessWidgetInfoAttributes(type);
                                        }
                                        catch(Exception exWidget)
                                        {
                                            Log.Warn("Widget", "Failed to load Widget {0}. Reason: {1}", type.FullName,exWidget.Message);
                                        }
                                    }
                                }

                                _widgets.Sort(
                                    delegate(WidgetDescription wi1, WidgetDescription wi2)
                                        {
                                            return Comparer<string>.Default.Compare(wi1.Name,wi2.Name);
                                        });
                            }
                            catch (ReflectionTypeLoadException rtle)
                            {
                                if (assemblies[i].IndexOf("DataBuddy") == -1 && assemblies[i].IndexOf("RssToolkit") == -1)
                                    Log.Warn("Widget", "Failed to load assembly {0}. Reason: {1}",
                                             assemblies[i], rtle.Message);
                            }

                            catch (Exception exAssembly)
                            {
                                Log.Warn("Widget", "Failed to load assembly {0}. Reason: {1}", assemblies[i], exAssembly.Message);
                            }
                        }

                        _isLoaded = true;
                    }
                }
            }
        }

        private static void ProcessWidgetInfoAttributes(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof (WidgetInfoAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                WidgetInfoAttribute wia = attributes[0] as WidgetInfoAttribute;
                if (wia != null)
                {
                    _widgets.Add(new WidgetDescription(wia.UniqueId,wia.Name,wia.Description,type));
                }
            }
        }
    }
}