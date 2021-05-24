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

		foreach(ObjectInfo player in SpawnPacket.Objects)
        {
			Managers.Object.Add(player, myPlayer: false);
        }
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn DespawnPacket = packet as S_Despawn;
		ServerSession serverSession = session as ServerSession;

		foreach (int id in DespawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move MovePacket = packet as S_Move;
		ServerSession serverSession = session as ServerSession;


		GameObject go = Managers.Object.FindById(MovePacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc == null)
			return;

		cc.PosInfo = MovePacket.PosInfo;
	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
		S_Skill skillPacket = packet as S_Skill;

		GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		if (go == null)
			return;

		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc == null)
			return;

		pc.UseSkill(skillPacket.Info.Skillid);

	}
}
