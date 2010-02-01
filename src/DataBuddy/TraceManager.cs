using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace DataBuddy
{
	public class TraceManager : ITrace
	{
		private static ITrace _Instance;
		private static readonly object _traceLocker = new object();
		private static bool _tracingEnabled = false;
		private const string DATABUDDY_ENABLE_TRACING = "DataBuddy::EnableTracing";

		#region Constructors

		static TraceManager()
		{
			bool tracingEnabled = false;

			if( bool.TryParse( ConfigurationManager.AppSettings[DATABUDDY_ENABLE_TRACING], out tracingEnabled ) )
			{
				_tracingEnabled = tracingEnabled;
			}
		}

		public TraceManager()
		{
			try
			{
				Initialize();
			}
			catch
			{
				_tracingEnabled = false;
				_Instance = null;
			}
		}

		#endregion

		#region private void Initialize()

		private void Initialize()
		{
			AssemblyName asmName = Assembly.GetExecutingAssembly().GetName();
			string logFileName = "DataBuddy.log"; //DateTime.Now.ToString("yyyyMMdd") + ".log";
			string currentPath = ( string.IsNullOrEmpty(HttpRuntime.AppDomainAppPath) )
				? Environment.CurrentDirectory
				: HttpRuntime.AppDomainAppPath;

			// enable tracing for this process.  Get the same name as this process with ".log" as the end.
			string logFilePath = Path.Combine(currentPath, logFileName);

			FileStream fs = new FileStream(logFilePath, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

			TextWriterTraceListener txtListener = new TextWriterTraceListener(sw, "txt_listener");

			Trace.Listeners.Add(txtListener);
			Trace.AutoFlush = true;

			Separator();

			Trace.WriteLine(asmName.FullName);

			Trace.Write("Application Domain Path: ");
			Trace.WriteLine(currentPath);

			Trace.Write("Started from: ");
			Trace.WriteLine(logFilePath);

			Trace.Write("Starting at: ");
			Trace.WriteLine(DateTime.Now.ToString("s"));

			Trace.Write("Executing Command Line: ");
			Trace.WriteLine(Environment.CommandLine);

			Trace.Write("Machine Name: ");
			Trace.WriteLine(Environment.MachineName);

			Trace.Write("OS Version: ");
			Trace.WriteLine(Environment.OSVersion);

			HttpContext httpContext = HttpContext.Current;
			if ( httpContext != null )
			{
				Trace.Write("Executing User: ");
				
				if ( httpContext.User != null )
					Trace.WriteLine(httpContext.User.Identity.Name);
				else
					Trace.WriteLine(Environment.UserName);
			}

			Trace.Write("Dot.Net Version: ");
			Trace.WriteLine(Environment.Version);

			Trace.Write("Current Data Provider: ");
			Trace.WriteLine(DataService.Provider.GetType().FullName);

			Separator();
		}

		#endregion

		#region public static ITrace Instance

		public static ITrace Instance
		{
			get
			{
				lock ( _traceLocker )
				{
					if ( _Instance == null )
					{
						_Instance = (_tracingEnabled) ? (ITrace)(new TraceManager()) : (ITrace)(new TraceEmpty());
					}

					return _Instance;
				}
			}
		}

		#endregion

		#region private string GetCurrentDateTimeString()

		private string GetCurrentDateTimeString()
		{
			return DateTime.Now.ToString( "yyyy-MM-dd hh:mm:ss.ffffff" ) + " - ";
		}

		#endregion

		#region public Guid GetMarker()

		public Guid GetMarker()
		{
			return Guid.NewGuid();
		}

		#endregion

		#region public void Write(...) overloads

		public void Write(Guid marker, DbCommand cmd)
		{
			string sql = cmd.CommandText;
			StringBuilder sb = new StringBuilder();

			sb.Append( GetCurrentDateTimeString() )
				.Append("Marker: ")
				.Append(marker.ToString("D"))
				.Append(", Current SQL hash: ")
				.AppendLine(sql.GetHashCode().ToString())
				.Append("\tStatement: ")
				.AppendLine(sql);
			if ( cmd.Parameters.Count > 0 )
			{
				sb.AppendLine("\tParameters:");
				foreach( DbParameter p in cmd.Parameters )
				{
					try
					{
						object o = ( p.Value is DBNull ) ? (object)"NULL" : (
							(p.Value == null) ? (object)"null" : p.Value
							);
						sb.AppendLine(string.Format("\t\tName = {0}, Type = {1}, Value = {2}", p.ParameterName, p.DbType, p.Value));
					}
					catch (Exception e)
					{
						Trace.WriteLine( string.Format("Error thrown: {0}{1}SQL statement: {2}", e.ToString(), Environment.NewLine, sql) );
					}
				}
			}
			Trace.Write(sb.ToString());
		}

		public void Write(Guid marker, string data)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( GetCurrentDateTimeString() )
				.Append("Marker: ")
				.Append(marker.ToString("D"))
				.Append(", Data: ")
				.Append(data);
			Trace.WriteLine(sb.ToString());
		}

		public void Write(Guid marker, string format, params object [] data)
		{
			Write(marker, string.Format(format, data));
		}

		#endregion

		#region public void Separator()

		public void Separator()
		{
			Trace.WriteLine(new System.String('*', 76));
		}

		#endregion
	}
}
