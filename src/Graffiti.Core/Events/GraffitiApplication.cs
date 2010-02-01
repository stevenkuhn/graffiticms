using System;
using System.ComponentModel;
using System.Text;
using System.Web.Routing;
using System.Xml;

namespace Graffiti.Core
{
	public class GraffitiApplication
	{
		private EventHandlerList Events = new EventHandlerList();

		#region Email Form

		private static object EmailBeforeSendOject = new object();
		private static object EmailAfterSentObject = new object();

		public event EmailTemplateHandler BeforeEmailSent
		{
			add { Events.AddHandler(EmailBeforeSendOject, value); }
			remove { Events.RemoveHandler(EmailBeforeSendOject, value); }
		}

		public event EmailTemplateHandler AfterEmailSent
		{
			add { Events.AddHandler(EmailBeforeSendOject, value); }
			remove { Events.RemoveHandler(EmailBeforeSendOject, value); }
		}

		internal void ExecuteBeforeEmailSent(EmailTemplate template)
		{
			EmailTemplateHandler handler = Events[EmailBeforeSendOject] as EmailTemplateHandler;
			if (handler != null)
				handler(template, EventArgs.Empty);
		}

		internal void ExecuteAfterEmailSent(EmailTemplate template)
		{
			EmailTemplateHandler handler = Events[EmailAfterSentObject] as EmailTemplateHandler;
			if (handler != null)
				handler(template, EventArgs.Empty);
		}

		#endregion

		#region Begin/End

		private static object BeginRequestObject = new object();
		private static object EndRequestObject = new object();
		private static object UrlRoutingAddObject = new object();

		/// <summary>
		/// Wires up an event to the ASP.Net BeginRequest Event
		/// </summary>
		public event EventHandler BeginRequest
		{
			add { Events.AddHandler(BeginRequestObject, value); }
			remove { Events.RemoveHandler(BeginRequestObject, value); }
		}

		/// <summary>
		/// Wires up an event to the ASP.Net EndRequest Event
		/// </summary>
		public event EventHandler EndRequest
		{
			add { Events.AddHandler(EndRequestObject, value); }
			remove { Events.RemoveHandler(EndRequestObject, value); }
		}

		private void ExecuteGenericHandler(object key, object sender, EventArgs e)
		{
			EventHandler eh = Events[key] as EventHandler;
			if (eh != null)
				eh(sender, e);
		}

		/// <summary>
		/// Wires up an event for adding Url Routes
		/// </summary>
		public event UrlRoutingEventHandler UrlRoutingAdd
		{
			add { Events.AddHandler(UrlRoutingAddObject, value); }
			remove { Events.RemoveHandler(UrlRoutingAddObject, value); }
		}


		/// <summary>
		/// Executes the BeginRequest Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ExecuteBeginRequest(object sender, EventArgs e)
		{
			ExecuteGenericHandler(BeginRequestObject, sender, e);
		}

		/// <summary>
		/// Executes the EndRequest event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ExecuteEndRequest(object sender, EventArgs e)
		{
			ExecuteGenericHandler(EndRequestObject, sender, e);
		}

		/// <summary>
		/// Executes the UrlRoutingAdd Event
		/// </summary>
		/// <param name="routeTable"></param>
		public void ExecuteUrlRoutingAdd(RouteCollection routes)
		{
			UrlRoutingEventHandler handler = Events[UrlRoutingAddObject] as UrlRoutingEventHandler;
			if (handler != null)
			{
				handler(routes, EventArgs.Empty);
			}
		}

		#endregion

		#region Graffiti Context

		private static object LoadGraffitiContextObject = new object();

		/// <summary>
		/// Wires up an event to execute after the GraffitiContext is loaded at the beginning of a request
		/// </summary>
		public event GraffitiContextEventHandler LoadGraffitiContext
		{
			add { Events.AddHandler(LoadGraffitiContextObject, value); }
			remove { Events.RemoveHandler(LoadGraffitiContextObject, value); }
		}

		/// <summary>
		/// Executes the LoadGraffitiContext event
		/// </summary>
		/// <param name="item"></param>
		internal void ExecuteLoadGraffitiContext(GraffitiContext context)
		{
			GraffitiContextEventHandler re = Events[LoadGraffitiContextObject] as GraffitiContextEventHandler;
			if (re != null)
			{
				re(context, EventArgs.Empty);
			}
		}

		#endregion

		#region RSS

		private static object RssNamespaceObject = new object();
        private static object RssChannelObject = new object();
		private static object RssItemObject = new object();


		/// <summary>
		/// Wires up an event to execute after the core RSS namespaces have been added to the feed
		/// </summary>
		public event RssEventHandler RssNamespace
		{
			add { Events.AddHandler(RssNamespaceObject, value); }
			remove { Events.RemoveHandler(RssNamespaceObject, value); }
		}

        /// <summary>
        /// Wires up an event to execute after the RSS channel has been added to the feed
        /// </summary>
        public event RssEventHandler RssChannel {
            add { Events.AddHandler(RssChannelObject, value); }
            remove { Events.RemoveHandler(RssChannelObject, value); }
        }

		/// <summary>
		/// Wires up an event to execute when a new RSSItem is added to a feed
		/// </summary>
		public event RssPostEventHandler RssItem
		{
			add { Events.AddHandler(RssItemObject, value); }
			remove { Events.RemoveHandler(RssItemObject, value); }
		}

		/// <summary>
		/// Executes the RssNamespace event
		/// </summary>
		/// <param name="item"></param>
		internal void ExecuteRssNamespace(XmlTextWriter writer)
		{
			RssEventHandler re = Events[RssNamespaceObject] as RssEventHandler;
			if (re != null)
			{
				re(writer, EventArgs.Empty);
			}
		}

        /// <summary>
        /// Executes the RssChannel event
        /// </summary>
        /// <param name="item"></param>
        internal void ExecuteRssChannel(XmlTextWriter writer) {
            RssEventHandler re = Events[RssChannelObject] as RssEventHandler;
            if (re != null) {
                re(writer, EventArgs.Empty);
            }
        }

		/// <summary>
		/// Executes the RssItem event
		/// </summary>
		/// <param name="item"></param>
		internal void ExecuteRssItem(XmlTextWriter writer, Post post)
		{
			RssPostEventHandler re = Events[RssItemObject] as RssPostEventHandler;
			if (re != null)
			{
				re(writer, new PostEventArgs(post, PostRenderLocation.Feed));
			}
		}

		#endregion

		#region Content

		private static object RenderHtmlHeaderObject = new object();
		private static object renderPostBodyObject = new object();

		/// <summary>
		/// Wires up an event to add content to the HTML Head section of the document
		/// </summary>
		public event RenderContentEventHandler RenderHtmlHeader
		{
			add { Events.AddHandler(RenderHtmlHeaderObject, value); }
			remove { Events.RemoveHandler(RenderHtmlHeaderObject, value); }
		}

		/// <summary>
		/// Wires up an event to append content to the post body
		/// </summary>
		public event RenderPostBodyEventHandler RenderPostBody
		{
			add { Events.AddHandler(renderPostBodyObject, value); }
			remove { Events.RemoveHandler(renderPostBodyObject, value); }
		}

		/// <summary>
		/// Executes the RenderHeader event
		/// </summary>
		/// <param name="sb">StringBuilder containing the current header content</param>
		internal void ExecuteRenderHtmlHeader(StringBuilder sb)
		{
			RenderContentEventHandler re = Events[RenderHtmlHeaderObject] as RenderContentEventHandler;
			if (re != null)
			{
				re(sb, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Executes the RenderPostBody event
		/// </summary>
		/// <param name="sb">StringBuilder containing the current content for Post.Body</param>
		/// <param name="post">Current Post</param>
		/// <param name="renderLocation">The location this event is executing</param>
		internal void ExecuteRenderPostBody(StringBuilder sb, Post post, PostRenderLocation renderLocation)
		{
			RenderPostBodyEventHandler re = Events[renderPostBodyObject] as RenderPostBodyEventHandler;
			if (re != null)
			{
				re(sb, new PostEventArgs(post, renderLocation));
			}
		}

		#endregion

		#region User

		private static object UserIsKnownObject = new object();
		private static object AfterNewUserObject = new object();
		private static object BeforeUserUpdateObject = new object();
		private static object AfterUserUpdateObject = new object();

		/// <summary>
		/// Wires up an event to execute as soon as the user is known
		/// </summary>
		public event UserEventHandler UserIsKnown
		{
			add { Events.AddHandler(UserIsKnownObject, value); }
			remove { Events.RemoveHandler(UserIsKnownObject, value); }
		}

		/// <summary>
		/// Wires up an event to execute after a new user is created
		/// </summary>
		public event UserEventHandler AfterNewUser
		{
			add { Events.AddHandler(AfterNewUserObject, value); }
			remove { Events.RemoveHandler(AfterNewUserObject, value); }
		}

		/// <summary>
		/// Wires up an event to execute before a IGraffitiUser is updated.
		/// </summary>
		public event UserEventHandler BeforeUserUpdate
		{
			add { Events.AddHandler(BeforeUserUpdateObject, value); }
			remove { Events.RemoveHandler(BeforeUserUpdateObject, value); }
		}

		/// <summary>
		/// Wires up an event to execute after an IGraffitiUser is updated
		/// </summary>
		public event UserEventHandler AfterUserUpdate
		{
			add { Events.AddHandler(AfterUserUpdateObject, value); }
			remove { Events.RemoveHandler(AfterUserUpdateObject, value); }
		}

		/// <summary>
		/// Executes the user is known event.
		/// </summary>
		/// <param name="user"></param>
		public void ExecuteUserIsKnown(IGraffitiUser user)
		{
			ExecuteUserEvent(UserIsKnownObject, user);
		}

		//public void ExecuteBeforeNewUser(IGraffitiUser user)
		//{
		//    ExecuteUserEvent(BeforeNewUserObject, user);
		//}

		/// <summary>
		/// Executes the AfterNewUser Event
		/// </summary>
		/// <param name="user"></param>
		internal void ExecuteAfterNewUser(IGraffitiUser user)
		{
			ExecuteUserEvent(AfterNewUserObject, user);
		}

		/// <summary>
		/// Executes the BeforeUserUpdate Event
		/// </summary>
		/// <param name="user"></param>
		internal void ExecuteUserBeforeUserUpdate(IGraffitiUser user)
		{
			ExecuteUserEvent(BeforeUserUpdateObject, user);
		}

		/// <summary>
		/// Executes the AfterUserUpdate event
		/// </summary>
		/// <param name="user"></param>
		internal void ExecuteAfterUserUpdated(IGraffitiUser user)
		{
			ExecuteUserEvent(AfterUserUpdateObject, user);
		}

		private void ExecuteUserEvent(object key, IGraffitiUser user)
		{
			UserEventHandler uv = Events[key] as UserEventHandler;
			if (uv != null)
			{
				uv(user, EventArgs.Empty);
			}
		}

		#endregion

		#region Data Objects

		private static object BeforeValidateObject = new object();
		private static object BeforeInsertObject = new object();
		private static object BeforeUpdateOjbect = new object();
		private static object AfterCommitObject = new object();
		private static object AfterInsertObject = new object();
		private static object AfterUpdateObject = new object();
		private static object BeforeRemoveObject = new object();
		private static object AfterRemoveObject = new object();
		private static object BeforeDestroyObject = new object();
		private static object AfterDestroyObject = new object();

		/// <summary>
		/// Wires up an event for the DataBuddy BeforeValidate Event
		/// </summary>
		public event DataObjectEventHandler BeforeValidate
		{
			add { Events.AddHandler(BeforeValidateObject, value); }
			remove { Events.RemoveHandler(BeforeValidateObject, value); }
		}

		/// <summary>
		/// Wires up an event for the DataBuddy BeforeInsert Event
		/// </summary>
		public event DataObjectEventHandler BeforeInsert
		{
			add { Events.AddHandler(BeforeInsertObject, value); }
			remove { Events.RemoveHandler(BeforeInsertObject, value); }
		}

		/// <summary>
		/// Wires up an event for the DataBuddy BeforeUpdate Event
		/// </summary>
		public event DataObjectEventHandler BeforeUpdate
		{
			add { Events.AddHandler(BeforeUpdateOjbect, value); }
			remove { Events.RemoveHandler(BeforeUpdateOjbect, value); }
		}

		/// <summary>
		/// Wires up an event for the DataBuddy AfterCommit Event
		/// </summary>
		public event DataObjectEventHandler AfterCommit
		{
			add { Events.AddHandler(AfterCommitObject, value); }
			remove { Events.RemoveHandler(AfterCommitObject, value); }
		}

		/// <summary>
		/// Wires up an event for the DataBuddy AfterInsert Event
		/// </summary>
		public event DataObjectEventHandler AfterInsert
		{
			add { Events.AddHandler(AfterInsertObject, value); }
			remove { Events.RemoveHandler(AfterInsertObject, value); }
		}

		/// <summary>
		/// Wires up an event for the DataBuddy AfterUpdate Event
		/// </summary>
		public event DataObjectEventHandler AfterUpdate
		{
			add { Events.AddHandler(AfterUpdateObject, value); }
			remove { Events.RemoveHandler(AfterUpdateObject, value); }
		}

		/// <summary>
		/// Wires up an event for the DataBuddy BeforeRemove Event
		/// </summary>
		public event DataObjectEventHandler BeforeRemove
		{
			add { Events.AddHandler(BeforeRemoveObject, value); }
			remove { Events.RemoveHandler(BeforeRemoveObject, value); }
		}

		/// <summary>
		/// Wires up an event for the DataBuddy AfterRemove Event
		/// </summary>
		public event DataObjectEventHandler AfterRemove
		{
			add { Events.AddHandler(AfterRemoveObject, value); }
			remove { Events.RemoveHandler(AfterRemoveObject, value); }
		}

		/// <summary>
		/// Wires up an event for the DataBuddy BeforeDestroy Event
		/// </summary>
		public event DataObjectEventHandler BeforeDestroy
		{
			add { Events.AddHandler(BeforeDestroyObject, value); }
			remove { Events.RemoveHandler(BeforeDestroyObject, value); }
		}

		/// <summary>
		/// Wires up an event for the DataBuddy AfterDestroy Event
		/// </summary>
		public event DataObjectEventHandler AfterDestroy
		{
			add { Events.AddHandler(AfterDestroyObject, value); }
			remove { Events.RemoveHandler(AfterDestroyObject, value); }
		}

		private void ExecuteDataObjectEvent(object key, DataBuddyBase dbb)
		{
			DataObjectEventHandler doe = Events[key] as DataObjectEventHandler;
			if (doe != null)
			{
				doe(dbb, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Executes the BeforeValidate Vent
		/// </summary>
		internal void ExecuteBeforeValidateEvent(DataBuddyBase dbb)
		{
			ExecuteDataObjectEvent(BeforeValidateObject, dbb);
		}

		/// <summary>
		/// Executes the BeforeInsert Vent
		/// </summary>
		internal void ExecuteBeforeInsertEvent(DataBuddyBase dbb)
		{
			ExecuteDataObjectEvent(BeforeInsertObject, dbb);
		}

		/// <summary>
		/// Executes the BeforeUpdate Vent
		/// </summary>
		internal void ExecuteBeforeUpdateEvent(DataBuddyBase dbb)
		{
			ExecuteDataObjectEvent(BeforeUpdateOjbect, dbb);
		}

		/// <summary>
		/// Executes the AfterCommit Vent
		/// </summary>
		internal void ExecuteAfterCommitEvent(DataBuddyBase dbb)
		{
			ExecuteDataObjectEvent(AfterCommitObject, dbb);
		}

		/// <summary>
		/// Executes the AfterInsert Vent
		/// </summary>
		internal void ExecuteAfterInsertEvent(DataBuddyBase dbb)
		{
			ExecuteDataObjectEvent(AfterInsertObject, dbb);
		}

		/// <summary>
		/// Executes the AfterUpdate Vent
		/// </summary>
		internal void ExecuteAfterUpdateEvent(DataBuddyBase dbb)
		{
			ExecuteDataObjectEvent(AfterUpdateObject, dbb);
		}

		/// <summary>
		/// Executes the BeforeRemove Vent
		/// </summary>
		internal void ExecuteBeforeRemoveEvent(DataBuddyBase dbb)
		{
			ExecuteDataObjectEvent(BeforeRemoveObject, dbb);
		}

		/// <summary>
		/// Executes the AfterRemove Vent
		/// </summary>
		internal void ExecuteAfterRemoveEvent(DataBuddyBase dbb)
		{
			ExecuteDataObjectEvent(AfterRemoveObject, dbb);
		}

		/// <summary>
		/// Executes the BeforeDestroy Vent
		/// </summary>
		internal void ExecuteBeforeDestroyEvent(DataBuddyBase dbb)
		{
			ExecuteDataObjectEvent(BeforeDestroyObject, dbb);
		}

		/// <summary>
		/// Executes the AfterDestroy Vent
		/// </summary>
		internal void ExecuteAfterDestroyEvent(DataBuddyBase dbb)
		{
			ExecuteDataObjectEvent(AfterDestroyObject, dbb);
		}

		#endregion

	}
}