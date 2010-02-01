using System;
using System.Data;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Xml.Serialization;
using DataBuddy;

namespace Graffiti.Core
{
    public static class ObjectManager
    {
        public static void Delete(string name)
        {
            ObjectStore os = ObjectStore.FetchByColumn(ObjectStore.Columns.Name, name);

            ObjectStore.Destroy(os.Id);

            ZCache.RemoveCache("object-" + name);
        }

        public static T Get<T>(string name) where T : class, new()
        {
            string cacheKey = "object-" + name;
            T t = ZCache.Get<T>(cacheKey);
            if(t == null)
            {
               ObjectStore os = ObjectStore.FetchByColumn(ObjectStore.Columns.Name, name);
               if(os.IsLoaded)
               {
                   t = ConvertToObject<T>(os.Data);
               }
               else
               {
                   t = new T();
               }

               if (t == null)
                   throw new Exception("Type " + typeof (T) + " could not be found or created"); 

                ZCache.MaxCache(cacheKey,t);
            }

            return t;
        }

        public static void Save(object objectToSave, string name)
        {
            ObjectStore os = ObjectStore.FetchByColumn(ObjectStore.Columns.Name, name);
            os.Data = ConvertToString(objectToSave);
           
            if (!os.IsLoaded)
            {
                os.ContentType = "xml/serialization";
                os.Name = name;
                os.Type = objectToSave.GetType().FullName;
                os.Version++;
            }

            os.Save();

            ZCache.RemoveCache("object-" + name);
            ZCache.InsertCache("object-" + name,objectToSave,120);
            
        }

        public static object ConvertToObject(string xml, Type type)
        {
            object convertedObject = null;

            if (!string.IsNullOrEmpty(xml))
            {
                using (StringReader reader = new StringReader(xml))
                {
                    XmlSerializer ser = new XmlSerializer(type);
                    try
                    {
                        convertedObject = ser.Deserialize(reader);
                    }
                    catch (InvalidOperationException) { }
                    reader.Close();
                }
            }
            return convertedObject;
        }

        public static T ConvertToObject<T>(string xml) where T : class, new()
        {
            T convertedObject = null;

            if (!string.IsNullOrEmpty(xml))
            {
                using (StringReader reader = new StringReader(xml))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(T));
                    try
                    {
                        convertedObject = ser.Deserialize(reader) as T;
                    }
                    catch (InvalidOperationException) { }
                    reader.Close();
                }
            }
            return convertedObject;
        }

        public static string ConvertToString(object objectToConvert)
        {
            string xml = null;

            if (objectToConvert != null)
            {
                //we need the type to serialize
                Type t = objectToConvert.GetType();

                XmlSerializer ser = new XmlSerializer(t);
                //will hold the xml
                using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
                {
                    ser.Serialize(writer, objectToConvert);
                    xml = writer.ToString();
                    writer.Close();
                }
            }

            return xml;
        }
    }

}