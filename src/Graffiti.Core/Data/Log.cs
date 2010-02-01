using System;
using DataBuddy;

namespace Graffiti.Core
{
    public partial class Log
    {
        protected override void AfterCommit()
        {
            
        }

        protected override void BeforeInsert()
        {
            
        }

        protected override void BeforeUpdate()
        {
            
        }

        protected override void BeforeValidate()
        {
            
        }

        public static void Info(string title, string messsage)
        {
            QuickSave(1,title,messsage);
        }

        public static void Info(string title, string messsageFormat, params object[] details)
        {
            DetailedSave(1,title,messsageFormat,details);
        }

        public static void Warn(string title, string messsage)
        {
            QuickSave(2, title, messsage);
        }

        public static void Warn(string title, string messsageFormat, params object[] details)
        {
            DetailedSave(2, title, messsageFormat, details);
        }

        public static void Error(string title, string messsage)
        {
            QuickSave(3, title, messsage);
        }

        public static void Error(string title, string messsageFormat, params object[] details)
        {
            DetailedSave(3, title, messsageFormat, details);
        }

        private static void DetailedSave(int type, string title, string messageFormat, params object[] details)
        {
            try
            {
                QuickSave(type, title, string.Format(messageFormat, details));
            }
            catch //need to make sure we throw no errors here
            {
            }
        }

        private static void QuickSave(int type, string title,string message)
        {
            Log l = new Log();
            l.Type = type;
            l.Title = title;
            l.Message = message;
            l.Save("", DateTime.Now.AddHours(SiteSettings.Get().TimeZoneOffSet));            
        }

        public static void RemoveLogsOlderThan(int hours)
        {
            DateTime dt = DateTime.Now.AddHours(-1*hours);

			QueryCommand command = new QueryCommand("DELETE FROM graffiti_Logs WHERE CreatedOn <= " + DataService.Provider.SqlVariable("CreatedOn"));
			command.Parameters.Add(Log.FindParameter("CreatedOn")).Value = dt;
            int i = DataService.ExecuteNonQuery(command);
            //if(i > 0)
            //    Info("Log", "{0} item(s) were just removed from the logs",i);
        }
    }
}
