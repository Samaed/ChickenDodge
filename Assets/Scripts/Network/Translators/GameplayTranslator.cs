using UnityEngine;
using System.Net;

public class GameplayTranslator {

	public static void Interpret(IPEndPoint source, Message message, GameplayMessage gameplay) {
		NetworkCommon networkCommon = GameObject.FindObjectOfType<Network> ().NetworkCommon;

        if (networkCommon == null) return;

        Player[] players = GameObject.FindObjectsOfType<Player>();

		switch (gameplay.Message) {
		    case GameplayMessage.MessageValue.MOVE:
			    foreach (Player player in players) {
				    if (player.ID == gameplay.PlayerID) {
					    player.Move(gameplay.MoveDelta, gameplay.OldPosition, !(networkCommon is NetworkServer));
				    }
			    }
			    break;
            case GameplayMessage.MessageValue.ATTACK:
                foreach (Player player in players) {
                    if (player.ID == gameplay.PlayerID)
                    {
                        player.Attack();
                    }
                }
                break;
		}
	}

	public static void Send(GameplayMessage.MessageValue messageValue, int id, Vector2 delta, Vector3 originalPosition) {
        Network network = GameObject.FindObjectOfType<Network>();

        if (network == null) return;

		NetworkCommon networkCommon = network.NetworkCommon;

        if (networkCommon == null) return;

		Message m = createMessage(	networkCommon.ID,
		                          	1-networkCommon.ID,
		                          	messageValue,
		                          	id,
		                          	delta,
                                    originalPosition );

		if (networkCommon is NetworkClient) {
			(networkCommon as NetworkClient).Send (null, MessageTranslator.ToByteArray (m));
		} else {
			(networkCommon as NetworkServer).Broadcast (MessageTranslator.ToByteArray (m));
		}

	}
	
	private static byte[] ToByteArray(GameplayMessage message) {
		return MessageTranslator.ToByteArray<GameplayMessage>(message);
	}

	private static Message createMessage(int sourceID, int destID, GameplayMessage.MessageValue message, int playerID, Vector2 delta, Vector3 originPosition) {
		Message m = new Message () {
			SourceID = sourceID,
			DestID = destID,
			Type = Message.MessageType.GAMEPLAY
		};
		
		GameplayMessage g = new GameplayMessage () {
			Message = message,
			PlayerID = playerID,
			MoveDelta = delta,
            OldPosition = originPosition
		};
		
		m.SerializedContent = ToByteArray(g);
		return m;
	}
}