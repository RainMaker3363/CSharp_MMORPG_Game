  
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

    public class PacketManager
    {
        #region Singleton
        static PacketManager _instance = new PacketManager();
        public static PacketManager Instance
        {
            get
            {
                return _instance;
            }
        }

        PacketManager()
        {
            Register();
        }
        #endregion

        Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _MakeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
        Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

        public void Register()
        {
      _MakeFunc.Add((ushort)ePacket.S_BroadcastEnterGame, MakePacket<S_BroadcastEnterGame>);
        _handler.Add((ushort)ePacket.S_BroadcastEnterGame, PacketHandler.S_BroadcastEnterGameHandler);

      _MakeFunc.Add((ushort)ePacket.S_BroadcastLeaveGame, MakePacket<S_BroadcastLeaveGame>);
        _handler.Add((ushort)ePacket.S_BroadcastLeaveGame, PacketHandler.S_BroadcastLeaveGameHandler);

      _MakeFunc.Add((ushort)ePacket.S_PlayerList, MakePacket<S_PlayerList>);
        _handler.Add((ushort)ePacket.S_PlayerList, PacketHandler.S_PlayerListHandler);

      _MakeFunc.Add((ushort)ePacket.S_BoradcastMove, MakePacket<S_BoradcastMove>);
        _handler.Add((ushort)ePacket.S_BoradcastMove, PacketHandler.S_BoradcastMoveHandler);


        }

        public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> OnRecvCallback = null)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            Func<PacketSession, ArraySegment<byte>, IPacket> _func = null;
            if(_MakeFunc.TryGetValue(id, out _func))
            {
                IPacket packet = _func.Invoke(session, buffer);
                if (OnRecvCallback != null)
                    OnRecvCallback.Invoke(session, packet);
                else
                    HandlePacket(session, packet);
            }
        }

        T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
        {
            T p = new T();
            p.Read(buffer);

            return p;
        }

        public void HandlePacket(PacketSession session, IPacket packet)
        {
            Action<PacketSession, IPacket> action = null;
            if (_handler.TryGetValue(packet.Protocol, out action))
            {
                action.Invoke(session, packet);
            }
        }
    }
