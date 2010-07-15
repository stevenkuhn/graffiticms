using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace GraffitiClient.API
{
    /// <summary>
    /// Root object for all resource proxies
    /// </summary>
    public abstract class ServiceProxy
    {
        private string _username;
        private string _password;
        private string _baseUrl;

        internal ServiceProxy(string username, string password, string baseUrl)
        {
            _username = username;
            _password = password;
            _baseUrl = baseUrl;
        }

        protected HttpWebRequest CreateRequest(string url, string xml, bool isDelete)
        {
            HttpWebRequest wreq = WebRequest.Create(url) as HttpWebRequest;
            SetAuthentication(wreq);

            if(xml != null)
            {
                if (isDelete)
                {
                    wreq.Headers.Add("Graffiti-Method", "DELETE");
                }

                wreq.ContentType = "application/x-www-form-urlencoded";
                wreq.Method = "POST";

                byte[] payload = Encoding.UTF8.GetBytes(xml);
                wreq.ContentLength = payload.Length;

                using (Stream st = wreq.GetRequestStream())
                {
                    st.Write(payload, 0, payload.Length);
                    st.Close();
                }
            }

            return wreq;

        }

        protected XmlDocument SendXML(string xml, bool isDelete)
        {
            HttpWebRequest wreq = CreateRequest(_baseUrl, xml, isDelete);
            return HandleRequest(wreq);

        }

        protected XmlDocument GetXML(NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder();
            if (nvc != null && nvc.Count > 0)
            {
                for (int i = 0; i < nvc.Count; i++)
                {
                    if (i == 0)
                        sb.Append("?");
                    else
                        sb.Append("&");

                    sb.AppendFormat("{0}={1}", nvc.AllKeys[i], HttpUtility.UrlEncode(nvc[i]));
                }
            }

            HttpWebRequest wreq = CreateRequest(_baseUrl + sb, null, false);
            return HandleRequest(wreq);

        }

        protected static XmlDocument HandleRequest(HttpWebRequest wreq)
        {
            try
            {
                HttpWebResponse httpResponse = wreq.GetResponse() as HttpWebResponse;
                XmlDocument doc = new XmlDocument();

                doc.Load(httpResponse.GetResponseStream());
                return doc;
            }
            catch (WebException webEx)
            {
                HttpWebResponse httpResponse = webEx.Response as HttpWebResponse;
                XmlDocument doc = new XmlDocument();
                doc.Load(httpResponse.GetResponseStream());

                XmlNode node = doc.SelectSingleNode("/error");
                if (node != null)
                    throw new GraffitiServiceException(node.InnerText, (int)httpResponse.StatusCode);
                else
                    throw new GraffitiServiceException("Unknown Graffiti Exception");
            }
        }

        protected static bool CheckResult (XmlDocument doc, string result)
        {
            int throwAwayId;

            return CheckResult(doc, result, out throwAwayId);
        }

        protected static bool CheckResult(XmlDocument doc, string result, out int id)
        {
            XmlNode node = doc.SelectSingleNode("/result");

            bool returnValue = (node != null && AreEqualIgnoreCase(node.InnerText, result));

            id = -1;
            if(node != null )
            {
                XmlAttribute idAttribute = node.Attributes["id"];
                if (idAttribute != null)
                    id = Int32.Parse(idAttribute.Value);
            }

            return returnValue;
        }

        protected virtual void SetAuthentication(HttpWebRequest wreq)
        {
            byte[] ba = Encoding.Default.GetBytes(_username + ":" + _password);
            string base64 = Convert.ToBase64String(ba);
            wreq.Headers.Add("Graffiti-Authorization", base64);
        }

        private static bool AreEqualIgnoreCase(string firstString, string secondString)
        {
            // if references match (or both are null), quickly return
            if (firstString == secondString) return true;

            // if one is null, return false
            if (firstString == null || secondString == null) return false;

            // with two different string instances, call Equals method 
            return firstString.Equals(secondString,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}