using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class GameplayMessage : ISerializable {

	public enum MessageValue : byte {
		MOVE,
        ATTACK
	}

	public MessageValue Message {
		get;
		set;
	}

	public int PlayerID {
		get;
		set;
	}

	public Vector2 MoveDelta {
		get;
		set;
	}

	public Vector3 OldPosition {
		get;
		set;
	}

	public GameplayMessage() {
	}
	
	public GameplayMessage(SerializationInfo info, StreamingContext context) {
		Message = (MessageValue)info.GetByte ("Message");
		PlayerID = info.GetInt32 ("PlayerID");
		MoveDelta = new Vector2((float)info.GetValue("x",typeof(float)),(float)info.GetValue("y",typeof(float)));
        OldPosition = new Vector3((float)info.GetValue("xloc", typeof(float)), (float)info.GetValue("yloc", typeof(float)), (float)info.GetValue("zloc", typeof(float)));
	}
	
	#region ISerializable implementation
	
	void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
	{
		info.AddValue("Message", Message, typeof(MessageValue));
		info.AddValue ("PlayerID", PlayerID, typeof(int));
		info.AddValue ("x", MoveDelta.x, typeof(float));
		info.AddValue ("y", MoveDelta.y, typeof(float));
        info.AddValue ("xloc", OldPosition.x, typeof(float));
		info.AddValue ("yloc", OldPosition.y, typeof(float));
        info.AddValue ("zloc", OldPosition.z, typeof(float));
	}
	
	#endregion

}
