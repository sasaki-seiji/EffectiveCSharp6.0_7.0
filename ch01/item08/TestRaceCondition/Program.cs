﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 2019.02.24 add
using System.Threading;

namespace TestRaceCondition
{
    public class EventSource
    {
        private EventHandler<int> Updated;
        public void SetHandler(EventHandler<int> h)
        {
            Updated = h;
        }
        public void ClearHandler()
        {
            Updated = null;
        }
        public void RaiseUpdates()
        {
            counter++;
            if (Updated != null)
                Updated(this, counter);
        }

        private int counter;
    }

    public class ClientThread
    {
        private const int SET_CLEAR_COUNT = 500;
        private const int LOOP_COUNT = 100;
        private const int sleep_ms = 100;

        private EventSource eventSource;
        public int LastCounter { get; set; }
        public bool StopRequest { get; set; }

        public ClientThread(EventSource es)
        {
            eventSource = es;
        }

        void Handler(Object sender, int param)
        {
            LastCounter = param;
        }

        public void exec()
        {
            for (int i = 0; i < LOOP_COUNT; ++i)
            {
                if (StopRequest) break;

                Thread.Sleep(sleep_ms);
                for (int j = 0; j < SET_CLEAR_COUNT; ++j)
                {
                    eventSource.ClearHandler();
                    eventSource.SetHandler(Handler);
                }
            }
        }
    }

    class Program
    {
        private const int LOOP_COUNT = 100;
        private const int RAISE_COUNT = 500;
        private const int sleep_ms = 100;

        static void Main(string[] args)
        {
            var source = new EventSource();
            var client = new ClientThread(source);

            Thread t = new Thread(new ThreadStart(client.exec));
            t.Start();

            try
            {
                for (int i = 0; i < LOOP_COUNT; ++i)
                {
                    Thread.Sleep(sleep_ms);
                    for (int j = 0; j < RAISE_COUNT; ++j)
                    {
                        source.RaiseUpdates();
                    }
                }
                Console.WriteLine("all done");
            }
            catch (Exception e)
            {
                Console.WriteLine("Execption occured: " + e);
                client.StopRequest = true;
            }
            Console.WriteLine("LastCounter: " + client.LastCounter);
            t.Join();
        }
    }
}
