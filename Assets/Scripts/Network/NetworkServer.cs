using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System;

public class NetworkServer : NetworkCommon
{
    List<IPEndPoint> m_Clients;

	public NetworkServer( int port )
		: this( new IPEndPoint( IPAddress.Any, port ) )
	{
		ID = 0;
	}

	public NetworkServer( IPEndPoint listenAddress ) : base(listenAddress)
	{
        m_Clients = new List<IPEndPoint>();
		m_Client.EnableBroadcast = true;
        Connected = true;
	}

	public override void Send( IPEndPoint target, byte[] data )
	{
		m_Client.Send( data, data.Length, target );
	}

	public void Broadcast( byte[] data ) {
        foreach (IPEndPoint client in m_Clients) {
		    Send (client, data);
        }
	}

    public void addClient(IPEndPoint client)
    {
        if (!m_Clients.Contains(client))
            m_Clients.Add(client);
    }
}
