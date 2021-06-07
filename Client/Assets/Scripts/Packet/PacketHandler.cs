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

		Managers.Object.Clear();
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

		// 다른 플레이어의 좌표만 갱신해줍니다.
		// 서버의 프레임으로 인해 좌표 밀려나는 현상을 방지
		if (Managers.Object.MyPlayer.Id == MovePacket.ObjectId)
			return;

		BaseController bc = go.GetComponent<BaseController>();
		if (bc == null)
			return;

		bc.PosInfo = MovePacket.PosInfo;
	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
		S_Skill skillPacket = packet as S_Skill;

		GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc == null)
			return;

		cc.UseSkill(skillPacket.Info.Skillid);

	}

	public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		S_ChangeHp changePacket = packet as S_ChangeHp;

		GameObject go = Managers.Object.FindById(changePacket.Objectid);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc == null)
			return;

		cc.Hp = changePacket.Hp;
	}

	public static void S_DieHandler(PacketSession session, IMessage packet)
	{
		S_Die diePacket = packet as S_Die;

		GameObject go = Managers.Object.FindById(diePacket.Objectid);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc == null)
			return;

		cc.Hp = 0;
		cc.OnDead();
	}
}
