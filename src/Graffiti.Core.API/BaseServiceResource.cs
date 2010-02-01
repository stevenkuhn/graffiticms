using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml;

namespace Graffiti.Core.API
{
    /// <summary>
    /// Base Page used to expose RESTful Resources. Primary responsiblity is Authentication, Invoking the response, and Error handling
    /// </summary>
    public abstract class BaseServiceResource : Page
    {
        protected virtual bool IsValidAccess(IGraffitiUser user)
        {
            return GraffitiUsers.IsAdmin(user);
            
        }

        protected override void OnLoad(EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "text/xml";

            string base64gheader = Request.Headers["Graffiti-Authorization"];

            if (!string.IsNullOrEmpty(base64gheader))
            {
                XmlTextWriter writer = new XmlTextWriter(Context.Response.OutputStream, Encoding.UTF8);
                try
                {
                    byte[] gheaderBytes = Convert.FromBase64String(base64gheader);
                    string rawValue = Encoding.Default.GetString(gheaderBytes);

                    int index = rawValue.IndexOf(":");

                    string username = rawValue.Substring(0, index);
                    string password = rawValue.Substring(index + 1);

                    IGraffitiUser gu = GraffitiUsers.Login(username, password, true);

                    if (gu != null)
                    {
                        if (IsValidAccess(gu))
                        {
                            HandleRequest(gu, writer);
                            writer.Close();
                        }
                        else
                        {
                            UnuathorizedRequest();
                        }

                        return;
                    }
                }
                catch(RESTConflict conflict)
                {
                    Response.StatusCode = 409;
                    writer.WriteElementString("error", conflict.Message);
                    writer.Close();
                    return;
                }
                catch (Exception ex)
                {
                    Response.StatusCode = 500;
                    writer.WriteElementString("error", ex.Message);
                    writer.Close();
                }
            }

            UnuathorizedRequest();

            base.OnLoad(e);
        }

        protected static int GetNodeValue(XmlNode node, int defaultValue)
        {
            if (node == null || string.IsNullOrEmpty(node.InnerText))
            {
                return defaultValue;
            }
            return Int32.Parse(node.InnerText.Trim());
        }

        protected static bool GetNodeValue(XmlNode node, bool defaultValue)
        {
            if (node == null || string.IsNullOrEmpty(node.InnerText))
            {
                return defaultValue;
            }
            return bool.Parse(node.InnerText.Trim());
        }

        protected static string GetNodeValue(XmlNode node, string defaultValue)
        {
            if (node == null || string.IsNullOrEmpty(node.InnerText))
            {
                return defaultValue;
            }
            return node.InnerText.Trim();
        }

        protected static DateTime GetNodeValue(XmlNode node, DateTime defaultValue)
        {
            if (node == null || string.IsNullOrEmpty(node.InnerText))
            {
                return defaultValue;
            }
            return DateTime.Parse(node.InnerText.Trim());
        }


        protected void UnuathorizedRequest()
        {
            Response.StatusCode = 403;

            Response.Status = "403 Invalid Credentials";
            
            Response.End();
        }

        protected abstract void HandleRequest(IGraffitiUser user, XmlTextWriter writer);

    }
}
