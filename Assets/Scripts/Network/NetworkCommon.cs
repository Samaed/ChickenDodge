using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public abstract class NetworkCommon
{
	public int ID {
		get;
		set;
	}

    public bool Connected
    {
        get;
        set;
    }

	public delegate void ReceiveDelegate( byte[] data, Action<byte[]> response );

	protected UdpClient m_Client;

	protected NetworkCommon() : this(null) {}

	protected NetworkCommon(IPEndPoint endPoint) {
		if (endPoint == null)
			m_Client = new UdpClient ();
		else
			m_Client = new UdpClient (endPoint);

		m_Client.Client.ReceiveBufferSize = 8192;
	}

	public abstract void Send( IPEndPoint target, byte[] data );

	public void Receive( ReceiveDelegate receiveCallback )
	{
		m_Client.BeginReceive( new AsyncCallback( ReceiveResult ), receiveCallback );
	}

	void ReceiveResult( IAsyncResult ar )
	{
		var receiveCallback = (ReceiveDelegate) ar.AsyncState;
		IPEndPoint ep = null;
		var bytes = m_Client.EndReceive( ar, ref ep );

        if (this is NetworkServer)
            (this as NetworkServer).addClient(ep);

		receiveCallback (bytes, b => MessageTranslator.Interpret (ep, b));
	}
}
