using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{

	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		ServerSession serverSession = session as ServerSession;

		Managers.Object.Add(enterGamePacket.Player, myPlayer:true);

		//Debug.Log("S_EnterGameHandle");
		//Debug.Log(enterGamePacket.Player);
	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame LeavePacket = packet as S_LeaveGame;
		ServerSession serverSession = session as ServerSession;

		Debug.Log("S_LeaveGameHandler");

		Managers.Object.RemoveMyPlayer();
	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn SpawnPacket = packet as S_Spawn;
		ServerSession serverSession = session as ServerSession;

		foreach(PlayerInfo player in SpawnPacket.Players)
        {
			Managers.Object.Add(player, myPlayer: false);
        }
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn DespawnPacket = packet as S_Despawn;
		ServerSession serverSession = session as ServerSession;

		foreach (int id in DespawnPacket.PlayerIds)
		{
			Managers.Object.Remove(id);
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move MovePacket = packet as S_Move;
		ServerSession serverSession = session as ServerSession;

		Debug.Log("S_MoveHandler");
	}
}
