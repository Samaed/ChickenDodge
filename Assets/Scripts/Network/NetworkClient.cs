using System.Net;
using System.Net.Sockets;

public class NetworkClient : NetworkCommon
{
	public NetworkClient( string host, int port )
	{
		ID = 1;
		m_Client.Connect( host, port );
	}

	public override void Send( IPEndPoint target, byte[] data )
	{
		Send( data );
	}

	public void Send( byte[] data )
	{
		m_Client.Send( data, data.Length );
	}
}
