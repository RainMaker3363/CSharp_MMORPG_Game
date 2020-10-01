using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public class ReceiveBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;

        public ReceiveBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize
        {
            get
            {
                return (_writePos - _readPos);
            }
        }

        public int FreeSize
        {
            get
            {
                return (_buffer.Count - _writePos);
            }
        }

        public ArraySegment<byte> ReadSegment
        {
            get
            {
                return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize);
            }
        }

        public ArraySegment<byte> WriteSegment
        {
            get
            {
                return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize);
            }
        }

        public void Clean()
        {
            int datasize = DataSize;
            if(datasize <= 0)
            {
                // 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                // 남은 찌끄레기가 있으면 시작 위치로 복사
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, datasize);
                _readPos = 0;
                _writePos = datasize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;


            _readPos += numOfBytes;

            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;

            _writePos += numOfBytes;

            return true;
        }
    }
}
