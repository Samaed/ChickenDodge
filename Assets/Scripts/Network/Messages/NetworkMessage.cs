using System;
using System.Runtime.Serialization;

[Serializable]
public class NetworkMessage  : ISerializable {

	public enum MessageValue : byte {
		CONNECT,
		CONNECTED,
		WRONG_MESSAGE
	};

	public MessageValue Message {
		get;
		set;
	}

	public NetworkMessage() {
	}

	public NetworkMessage(SerializationInfo info, StreamingContext context) {
		Message = (MessageValue)info.GetByte ("Message");
	}

	#region ISerializable implementation

	void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
	{
		info.AddValue("Message", Message, typeof(MessageValue));
	}

	#endregion
}
