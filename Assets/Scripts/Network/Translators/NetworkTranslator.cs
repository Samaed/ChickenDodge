using UnityEngine;
using System.Net;

public class NetworkTranslator
{
	public static void Interpret(IPEndPoint source, Message message, NetworkMessage network) {
		NetworkCommon networkCommon = GameObject.FindObjectOfType<Network> ().NetworkCommon;
		switch (network.Message) {
		case NetworkMessage.MessageValue.CONNECT:
			if (networkCommon is NetworkServer && networkCommon.ID == message.DestID) {
				(networkCommon as NetworkServer).Send(source, MessageTranslator.ToByteArray(
					createMessage(	networkCommon.ID,
				              		message.SourceID,
				              		NetworkMessage.MessageValue.CONNECTED )));
                networkCommon.Connected = true;
				Application.LoadLevel(1);
			}
			break;
		case NetworkMessage.MessageValue.CONNECTED:
			if (networkCommon is NetworkClient && networkCommon.ID == message.DestID) {
                networkCommon.Connected = true;
				Application.LoadLevel(1);
			}
			break;
		}
	}

	public static byte[] ToByteArray(NetworkMessage message) {
		return MessageTranslator.ToByteArray<NetworkMessage>(message);
	}

	public static Message createMessage(int sourceID, int destID, NetworkMessage.MessageValue message) {
		Message m = new Message() {
			SourceID = sourceID,
			DestID = destID,
			Type = Message.MessageType.NETWORK
		};
		
		NetworkMessage n = new NetworkMessage() {
			Message = message
		};
		
		m.SerializedContent = ToByteArray(n);
		return m;
	}
}