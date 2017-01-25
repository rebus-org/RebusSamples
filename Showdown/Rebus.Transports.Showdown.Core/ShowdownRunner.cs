using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Bus;
using Timer = System.Timers.Timer;

#pragma warning disable 1998

namespace Rebus.Transports.Showdown.Core
{
    public class ShowdownRunner : IDisposable
    {
        const int MessageCount = 10000;
        const int NumberOfWorkers = 5;

        readonly BuiltinHandlerActivator _adapter = new BuiltinHandlerActivator();

        public BuiltinHandlerActivator Adapter => _adapter;

        public async Task Run(string showdownName)
        {
            try
            {
                using (var printTimer = new Timer())
                {
                    var sentMessagesCount = 0;
                    var receivedMessagesCount = 0;
                    printTimer.Interval = 5000;
                    printTimer.Elapsed +=
                        delegate
                        {
                            Print("Sent {0} messages. Received {1} messages.", sentMessagesCount, receivedMessagesCount);
                        };
                    printTimer.Start();

                    Print(@"----------------------------------------------------------------------
Running showdown: {0}
----------------------------------------------------------------------",
                        showdownName);
                    // .GetName()
                    // .Name);

                    var receivedMessageIds = new ConcurrentDictionary<int, int>();
                    var receivedMessages = 0;

                    Print("Stopping all workers in receiver");
                    var receiverBus = (RebusBus)_adapter.Bus;
                    receiverBus.SetNumberOfWorkers(0);

                    Thread.Sleep(TimeSpan.FromSeconds(2));

                    Print("Sending {0} messages from sender to receiver", MessageCount);

                    var senderWatch = Stopwatch.StartNew();

                    await Task.WhenAll(Enumerable.Range(0, MessageCount)
                     .Select(async i =>
                        {
                            var message = new TestMessage { MessageId = i };
                            receivedMessageIds[message.MessageId] = 0;
                            await _adapter.Bus.SendLocal(message);
                            Interlocked.Increment(ref sentMessagesCount);
                        }));

                    var totalSecondsSending = senderWatch.Elapsed.TotalSeconds;
                    Print("Sending {0} messages took {1:0.0} s ({2:0.0} msg/s)",
                        MessageCount, totalSecondsSending, MessageCount / totalSecondsSending);

                    var resetEvent = new ManualResetEvent(false);

                    _adapter.Handle<TestMessage>(async message =>
                    {
                        var result = Interlocked.Increment(ref receivedMessages);
                        if (result == MessageCount)
                        {
                            resetEvent.Set();
                        }
                        Interlocked.Increment(ref receivedMessagesCount);
                    });


                    Print("Starting receiver with {0} workers", NumberOfWorkers);

                    var receiverWatch = Stopwatch.StartNew();
                    receiverBus.Advanced.Workers.SetNumberOfWorkers(NumberOfWorkers);

                    resetEvent.WaitOne();
                    var totalSecondsReceiving = receiverWatch.Elapsed.TotalSeconds;

                    Thread.Sleep(2000);
                    printTimer.Stop();
                    Print("Receiving {0} messages took {1:0.0} s ({2:0.0} msg/s)",
                        MessageCount, totalSecondsReceiving, MessageCount / totalSecondsReceiving);

                }
            }
            catch (Exception e)
            {
                Print("Error: {0}", e);
            }
        }

        void Print(string message, params object[] objs)
        {
            Console.WriteLine(message, objs);
        }

        public void Dispose()
        {
            if (_disposing || _disposed) return;

            lock (this)
            {
                if (_disposing || _disposed) return;

                try
                {
                    _disposing = true;
                    _adapter.Dispose();
                }
                finally
                {
                    _disposed = true;
                    _disposing = false;
                }
            }
        }

        bool _disposed;
        bool _disposing;


    }
}
