using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DataBuddy
{
	public class TraceEmpty : ITrace
	{
		public Guid GetMarker()
		{
			return Guid.Empty;
		}

		public void Write(Guid marker, DbCommand cmd) { }
		public void Write(Guid marker, string data) { }
		public void Write(Guid marker, string format, params object[] data) { }
		public void Separator() { }
	}
}
