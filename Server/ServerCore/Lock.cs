using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    #region 스핀 락
    /*
    class SpinLock
    {
        volatile int _Locked = 0;

        public void Accuire()
        {
            while (true)
            {
                // CAS Compare-And-Swap
                int excpected = 0;
                int desired = 1;

                if (Interlocked.CompareExchange(ref _Locked, desired, excpected) == excpected)
                    break;

                // 쉬다 올게!
                //Thread.Sleep(1); // 무조건 휴식 => 무조건 1ms 쉬고 싶어요
                //Thread.Sleep(0); // 조건부 양보 => 나보다 우선순위가 낮은 애들한테는 양보 불가 => 우선순위가 나보다 같거나 높은 쓰레드가 없으면 다시 본인한테
                Thread.Yield(); // 관대한 양보 => 관대하게 양보할테니, 지금 실행이 가능한 쓰레드가 있으면 실행하세요 => 실행 가능한 애가 없으면 남은 시간 소진
            }

        }

        public void Release()
        {
            _Locked = 0;
        }
    }
    */
    #endregion

    #region AutoResetLock
    /*
    class Lock
    {
        AutoResetEvent _available = new AutoResetEvent(true);

        public void Accuire()
        {
            _available.WaitOne();   // 입장 시도
        }

        public void Release()
        {
            _available.Set();

        }
    }
    */
    #endregion


    // 재귀적 락 허용 
    // 스핀락 정책(5000번 -> Yield)
    class Lock
    {
        const int EMPTY_FLAG        = 0x00000000;
        const int WRITE_MASK        = 0x7FFF0000;
        const int READ_MASK         = 0x0000FFFF;
        const int MAX_SPIN_COUNT    = 5000;

        // [Unused(1)] [WriteThreadId(15비트)] [ReadCount(16비트)]
        int _flag = EMPTY_FLAG;
        int _writeCount = 0;

        public void WriteLock()
        {
            // 동일 쓰레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if(Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                ++_writeCount;
                return;
            }

            // 아무도 WriteLock or ReadLock을 획득하고 있지 않을 때, 경합해서 소유권을 얻는다.
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            while(true)
            {
                for(int i = 0; i< MAX_SPIN_COUNT; ++i)
                {
                    // 시도를 해서 성공하면 Return
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                }

                Thread.Yield();
            }
        }

        public void WriteUnLock()
        {
            int lockCount = --_writeCount;
            if(lockCount <= 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Decrement(ref _flag);
                return;
            }

            // 아무도 WriteLock을 획득하고 있지 않으면, ReadCount를 1을 늘린다.
            while (true)
            {
                for(int i = 0; i < MAX_SPIN_COUNT; ++i)
                {
                    int expeceted = (_flag & WRITE_MASK);
                    if (Interlocked.CompareExchange(ref _flag, expeceted + 1, expeceted) == expeceted)
                        return;
                }

                Thread.Yield();
            }
        }

        public void ReadUnLock()
        {
            Interlocked.Decrement(ref _flag);
        }
    }
}
