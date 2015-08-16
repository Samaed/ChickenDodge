using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

public class Network : MonoBehaviour
{
	Queue<KeyValuePair<byte[], Action<byte[]>>> m_MessageQueue = new Queue<KeyValuePair<byte[], Action<byte[]>>>();

	public NetworkCommon NetworkCommon {
		get;
		private set;
	}

	public void Awake()
	{
		DontDestroyOnLoad( gameObject );
	}

	public void StartServer( int port )
	{
		NetworkCommon = new NetworkServer( port );
		NetworkCommon.Receive( OnDataReceived );
	}

	public void StartClient( string host, int port )
	{
		NetworkCommon = new NetworkClient( host, port );
		NetworkCommon.Receive( OnDataReceived );

        Invoke("ClientConnection", 0);
	}

    private void ClientConnection()
    {
        if (!NetworkCommon.Connected)
            Invoke("ClientConnection", .1f);

        NetworkCommon.Send(null, MessageTranslator.ToByteArray(
                NetworkTranslator.createMessage(NetworkCommon.ID, 1 - NetworkCommon.ID, NetworkMessage.MessageValue.CONNECT)
                ));

    }

	public void Update()
	{
		lock ( m_MessageQueue )
		{
			while ( m_MessageQueue.Count > 0 )
			{
				var msg = m_MessageQueue.Dequeue();
				HandleMessage( msg.Key, msg.Value );
			}
		}
	}

	void OnDataReceived( byte[] data, Action<byte[]> response )
	{
		m_MessageQueue.Enqueue( new KeyValuePair<byte[], Action<byte[]>>( data, response ) );
		NetworkCommon.Receive( OnDataReceived );
	}

	void HandleMessage( byte[] data, Action<byte[]> replyMethod )
	{
		replyMethod( data );
	}
}