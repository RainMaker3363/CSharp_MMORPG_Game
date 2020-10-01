using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;

namespace ServerCore
{
    public  class Listener
    {
        Socket _listenSocket;
        Func<Session> _SessionFactory;

        public void init(IPEndPoint endPoint, Func<Session> SessionFactory, int register = 10, int backlog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _SessionFactory += SessionFactory;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업 시작
            // backLog : 최대 대기수
            _listenSocket.Listen(backlog);
            
            for(int i = 0; i<register; ++i)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            // 기존 연결되어 있는 잔재를 정리합니다.
            args.AcceptSocket = null;

            // 대기 중 바로 접속이 완료되었다면 false를 뱉습니다.
            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
            {
                OnAcceptCompleted(null, args);
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = _SessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            // 처리 후 다시 등록을 합니다.
            RegisterAccept(args);
        }
    }

}
