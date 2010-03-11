using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Xml;
using DataBuddy;

namespace Graffiti.Core
{


	#region Event Handler Subscriptions
	public delegate void DataObjectEventHandler(DataBuddyBase dataObject, EventArgs e);
	public delegate void UserEventHandler(IGraffitiUser user, EventArgs e);
	public delegate void RssEventHandler(XmlTextWriter writer, EventArgs e);
	public delegate void RssPostEventHandler(XmlTextWriter writer, PostEventArgs e);
	public delegate void EmailTemplateHandler(EmailTemplate template, EventArgs e);
	public delegate void RenderContentEventHandler(StringBuilder sb, EventArgs e);
	public delegate void RenderPostBodyEventHandler(StringBuilder sb, PostEventArgs e);
	public delegate void GraffitiContextEventHandler(GraffitiContext context, EventArgs e);
	public delegate void UrlRoutingEventHandler(RouteCollection routes, EventArgs e);
	#endregion

	/// <summary>
	/// Manages events
	/// </summary>
	public static class Events
	{
		/// <summary>
		/// Gets an event from the ObjectStore or creates a new one if it doesn't exist
		/// </summary>
		public static EventDetails GetEvent(string typeName)
		{
			ObjectStore os = GetEventFromStore(typeName);
			EventDetails ed = null;
			if (os.IsNew)
			{
				ed = CreateNewEventFromTypeName(typeName);
			}
			else
			{
				ed = LoadEventDetailsFromObjectStore(os);
			}
			return ed;
		}
		/// <summary>
		/// Saves an EventDetails to the ObjectStore
		/// </summary>
		/// <param name="ed"></param>
		public static void Save(EventDetails ed)
		{
			ed.Xml = ObjectManager.ConvertToString(ed.Event);

			ObjectStore os = GetEventFromStore(ed.EventType);
			os.Name = ed.EventType;
			os.ContentType = "eventdetails/xml";
			os.Data = ObjectManager.ConvertToString(ed);
			os.Type = ed.GetType().ToString();
			os.Save(GraffitiUsers.Current.Name);

			ResetCache();
		}

		/// <summary>
		/// Clears the Event cache.
		/// </summary>
		public static void ResetCache()
		{
			ZCache.RemoveCache("EventDetails");
			ZCache.RemoveCache("GraffitiApplication");
		}



		/// <summary>
		/// Manages a single instance of GraffitiApplication which controls the events to be invoked
		/// </summary>
		public static GraffitiApplication Instance()
		{
			GraffitiApplication ga = ZCache.Get<GraffitiApplication>("GraffitiApplication");
			if (ga == null)
			{
				lock (lockedOnly)
				{
					ga = ZCache.Get<GraffitiApplication>("GraffitiApplication");
					if (ga == null)
					{
						ga = new GraffitiApplication();

						List<EventDetails> details = GetEvents();

						foreach (EventDetails detail in details)
						{
							if (detail.Enabled)
							{
								detail.Event.Init(ga);
							}
						}

						ZCache.InsertCache("GraffitiApplication", ga, 1800);
					}
				}
			}
			return ga;
		}

		/// <summary>
		/// Returns a list of all known events based the assemblies in the bin directory
		/// </summary>
		/// <returns></returns>
		public static List<EventDetails> GetEvents()
		{
			List<EventDetails> details = ZCache.Get<List<EventDetails>>("EventDetails");
			if (details == null)
			{
				details = new List<EventDetails>();

				ObjectStoreCollection osc =
					 ObjectStoreCollection.FetchByColumn(ObjectStore.Columns.ContentType, "eventdetails/xml");


				string[] assemblies =
					 Directory.GetFileSystemEntries(HttpRuntime.BinDirectory, "*.dll");
				for (int i = 0; i < assemblies.Length; i++)
				{
					try
					{
						Assembly asm = Assembly.LoadFrom(assemblies[i]);
						foreach (Type type in asm.GetTypes())
						{
							try
							{
								if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(GraffitiEvent)))
								{
									string the_Type = type.AssemblyQualifiedName;
									the_Type = the_Type.Substring(0, the_Type.IndexOf(", Version="));
									EventDetails ed = null;
									foreach (ObjectStore os in osc)
									{
										if (os.Name == the_Type)
										{
											ed = LoadEventDetailsFromObjectStore(os);
											break;
										}
									}

									if (ed == null)
									{
										ed = CreateNewEventFromTypeName(the_Type);
									}

									details.Add(ed);
								}
							}
							catch (Exception exType)
							{
								Log.Warn("Plugin", "Failed to load type {0}. Reason: {1}", type.FullName, exType.Message);
							}
						}

					}
					catch (ReflectionTypeLoadException rtle)
					{
						if (assemblies[i].IndexOf("DataBuddy") == -1 && assemblies[i].IndexOf("RssToolkit") == -1)
							Log.Warn("Plugin", "Failed to load assembly {0}. Reason: {1}",
										assemblies[i], rtle.Message);
					}

					catch (Exception exAssembly)
					{
						Log.Warn("Plugin", "Failed to load assembly {0}. Reason: {1}", assemblies[i], exAssembly.Message);
					}
				}

				ZCache.InsertCache("EventDetails", details, 300);
			}

			return details;
		}


		#region Private Helpers

		/// <summary>
		/// Returns instance of ObjectStore for a specific type name
		/// </summary>
		private static ObjectStore GetEventFromStore(string typeName)
		{
			Query q = ObjectStore.CreateQuery();
			q.AndWhere(ObjectStore.Columns.Name, typeName);
			q.AndWhere(ObjectStore.Columns.ContentType, "eventdetails/xml");
			return ObjectStore.FetchByQuery(q);
		}

		/// <summary>
		/// Creates an EventDetail from the ObjectStore
		/// </summary>
		private static EventDetails LoadEventDetailsFromObjectStore(ObjectStore os)
		{
			EventDetails ed = ObjectManager.ConvertToObject<EventDetails>(os.Data);
			ed.Event = InstantiateGraffitiEvent(ed.EventType, ed.Xml);

			return ed;
		}

		/// <summary>
		/// Events need some "help" deserializing. This method handles this.
		/// </summary>
		private static GraffitiEvent InstantiateGraffitiEvent(string typeName, string xml)
		{
			Type type = Type.GetType(typeName);
			if (type == null)
				throw new Exception("The type name " + typeName + " is not a valid type");

			if (string.IsNullOrEmpty(xml))
				return Activator.CreateInstance(type) as GraffitiEvent;
			return ObjectManager.ConvertToObject(xml, type) as GraffitiEvent;
		}

		/// <summary>
		/// Creates a new EventDetails and instantiates the Graffiti event for the give Type Name
		/// </summary>
		private static EventDetails CreateNewEventFromTypeName(string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type == null)
				throw new Exception("The type " + typeName + " is not valid");

			EventDetails ed = new EventDetails();
			ed.EventType = typeName;
			ed.Enabled = type.Assembly == typeof(Events).Assembly;
			ed.Event = InstantiateGraffitiEvent(typeName, null);
			return ed;
		}

		/// <summary>
		/// used to ensure that only one instance of GraffitiEvents exists
		/// </summary>
		private static readonly object lockedOnly = new object();

		#endregion

	}

}
