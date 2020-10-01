
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using ServerCore;

public enum ePacket
{
    S_BroadcastEnterGame = 1,
	C_LeaveGame = 2,
	S_BroadcastLeaveGame = 3,
	S_PlayerList = 4,
	C_Move = 5,
	S_BoradcastMove = 6,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


public class S_BroadcastEnterGame : IPacket
{
    public int playerid;
	public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get { return (ushort)ePacket.S_BroadcastEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        

        count += sizeof(ushort);                
        count += sizeof(ushort);

        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;


        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)ePacket.S_BroadcastEnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));

        count += sizeof(ushort);
     
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_LeaveGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)ePacket.C_LeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        

        count += sizeof(ushort);                
        count += sizeof(ushort);

        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;


        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)ePacket.C_LeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));

        count += sizeof(ushort);
     
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BroadcastLeaveGame : IPacket
{
    public int playerid;

    public ushort Protocol { get { return (ushort)ePacket.S_BroadcastLeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        

        count += sizeof(ushort);                
        count += sizeof(ushort);

        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;


        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)ePacket.S_BroadcastLeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));

        count += sizeof(ushort);
     
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_PlayerList : IPacket
{
    
	public class Player
	{
	    public bool isSelf;
		public int playerid;
		public float posX;
		public float posY;
		public float posZ;
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool bSuccess = true;
	
	        
	
	        return bSuccess;
	    }
	}
	
	public List<Player> players = new List<Player>();
	

    public ushort Protocol { get { return (ushort)ePacket.S_PlayerList; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        

        count += sizeof(ushort);                
        count += sizeof(ushort);

        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;


        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)ePacket.S_PlayerList), 0, segment.Array, segment.Offset + count, sizeof(ushort));

        count += sizeof(ushort);
     
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_Move : IPacket
{
    public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get { return (ushort)ePacket.C_Move; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        

        count += sizeof(ushort);                
        count += sizeof(ushort);

        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;


        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)ePacket.C_Move), 0, segment.Array, segment.Offset + count, sizeof(ushort));

        count += sizeof(ushort);
     
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BoradcastMove : IPacket
{
    public int playerid;
	public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get { return (ushort)ePacket.S_BoradcastMove; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        

        count += sizeof(ushort);                
        count += sizeof(ushort);

        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;


        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)ePacket.S_BoradcastMove), 0, segment.Array, segment.Offset + count, sizeof(ushort));

        count += sizeof(ushort);
     
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
