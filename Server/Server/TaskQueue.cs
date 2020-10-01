﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    interface ITask
    {
        void Execute();
    }
    class BroadcastTask : ITask
    {
        GameRoom _room;
        ClientSession _session;
        string _chat;

        BroadcastTask(GameRoom room, ClientSession session, string chat)
        {
            this._room = room;
            this._session = session;
            this._chat = chat;
        }
        public void Execute()
        {
            //_room.Boradcast(_session, _chat);
        }
    }

    class TaskQueue
    {
        Queue<ITask> _queue = new Queue<ITask>();
    }
}
