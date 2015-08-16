using System;
using System.Runtime.Serialization;

[Serializable]
public class Message : ISerializable {

	// TODO messages
	/*
	 *  | SourceID | DestID | MessageTypeID | TypeID | SerializedContent
	 *  MessageTypeID == Connect / Play
	 *  TypeID == MovePlayer / AttackPlayer ( / MoveChicken )
	 * 
	 *  | 0 (server) | 1 (dest) | 0 | 
	 * 
	 */

	public enum MessageType : byte {
		NETWORK,
		GAMEPLAY
	}
	
	public int SourceID {
		get;
		set;
	}

	public int DestID {
		get;
		set;
	}

	public MessageType Type {
		get;
		set;
	}

	public byte[] SerializedContent {
		get;
		set;
	}

	public Message() {
	}
	
	public Message(SerializationInfo info, StreamingContext context) {
		SourceID = info.GetInt32 ("SourceID");
		DestID = info.GetInt32 ("DestID");
		Type = (MessageType)info.GetByte ("MessageType");
		SerializedContent = (byte[])info.GetValue ("SerializedContent", typeof(byte[]));
	}

	#region ISerializable implementation
	void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("SourceID", SourceID, typeof(int));
		info.AddValue ("DestID", DestID, typeof(int));
		info.AddValue ("MessageType", Type, typeof(MessageType));
		info.AddValue ("SerializedContent", SerializedContent, typeof(byte[]));
	}
	#endregion

}
