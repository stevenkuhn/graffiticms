using System;
using System.Text;
using CookComputing.XmlRpc;

namespace Graffiti.BlogExtensions
{
	/// <summary>
	/// PingBack Interface
	/// See: http://www.hixie.ch/specs/pingback/pingback
	/// See: http://ln.hixie.ch/?start=1033171507&count=1
	/// </summary>
	public interface IPingBack
	{
		[XmlRpcMethod("pingback.ping", Description = "Notifies the server that a link has been added to sourceURI, pointing to targetURI.")]
		[return: XmlRpcReturnValue(Description = "A Message String")]
		string Ping(string sourceURI, string targetURI);
	}
}
