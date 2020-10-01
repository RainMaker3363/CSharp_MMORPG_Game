using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;


class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame chatPacket = packet as S_BroadcastEnterGame;
        ServerSession severSession = session as ServerSession;

        //if (chatPacket.playerid == 1)
        //Console.WriteLine(chatPacket.chat);
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeaveGame chatPacket = packet as S_BroadcastLeaveGame;
        ServerSession severSession = session as ServerSession;

        //if (chatPacket.playerid == 1)
        //Console.WriteLine(chatPacket.chat);
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList chatPacket = packet as S_PlayerList;
        ServerSession severSession = session as ServerSession;

        //if (chatPacket.playerid == 1)
        //Console.WriteLine(chatPacket.chat);
    }

    public static void S_BoradcastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BoradcastMove chatPacket = packet as S_BoradcastMove;
        ServerSession severSession = session as ServerSession;

        //if (chatPacket.playerid == 1)
        //Console.WriteLine(chatPacket.chat);
    }
}