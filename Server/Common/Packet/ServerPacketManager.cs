  
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
      _MakeFunc.Add((ushort)ePacket.C_LeaveGame, MakePacket<C_LeaveGame>);
        _handler.Add((ushort)ePacket.C_LeaveGame, PacketHandler.C_LeaveGameHandler);

      _MakeFunc.Add((ushort)ePacket.C_Move, MakePacket<C_Move>);
        _handler.Add((ushort)ePacket.C_Move, PacketHandler.C_MoveHandler);


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
