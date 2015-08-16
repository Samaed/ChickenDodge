using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System;

public class MessageTranslator {

	private static MemoryStream stream = new MemoryStream();
	private static BinaryFormatter formatter = new BinaryFormatter();
	private static Message message;

	public static Message Read(byte[] bytes) {
		return Read<Message> (bytes);
	}

	public static T Read<T>( byte[] bytes ) {
        stream.SetLength(0);
		stream.Seek (0, SeekOrigin.Begin);
		stream.Write (bytes, 0, bytes.Length);
		stream.Seek (0, SeekOrigin.Begin);

        return (T)formatter.Deserialize(stream);
	}

	public static byte[] ToByteArray(Message message) {
		return ToByteArray<Message> (message);
	}

	public static byte[] ToByteArray<T>( T message ) {
		stream.SetLength(0);
		stream.Seek (0, SeekOrigin.Begin);
		formatter.Serialize (stream, message);

		return stream.ToArray ();
	}

	public static void Interpret(IPEndPoint source, byte[] bytes) {
		try {
			message = Read (bytes);
		} catch (Exception ex) {
            Debug.Log(ex);
			return;
		}

		if (message.SourceID == GameObject.FindObjectOfType<Network> ().NetworkCommon.ID) {
			// TODO message sent by self returned
		} else {
			if (message.Type == Message.MessageType.GAMEPLAY)
				GameplayTranslator.Interpret(source, message, Read<GameplayMessage>(message.SerializedContent));
			else if (message.Type == Message.MessageType.NETWORK)
				NetworkTranslator.Interpret(source, message, Read<NetworkMessage>(message.SerializedContent));
		}
	}
}
