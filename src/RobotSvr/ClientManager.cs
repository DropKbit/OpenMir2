using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using SystemModule;

namespace RobotSvr
{
    public static class ClientManager
    {
        private static readonly ConcurrentDictionary<string, RobotClient> _Clients;
        private static int g_dwProcessTimeMin = 0;
        private static int g_dwProcessTimeMax = 0;
        private static int g_nPosition = 0;
        private static int dwRunTick = 0;
        private static readonly Channel<RecvicePacket> _reviceMsgList;

        static ClientManager()
        {
            _Clients = new ConcurrentDictionary<string, RobotClient>();
            _reviceMsgList = Channel.CreateUnbounded<RecvicePacket>();
        }

        public static void Start()
        {
            Task.Run(() =>
            {
                Task.Factory.StartNew(ProcessReviceMessage);
            });
        }

        private static async Task ProcessReviceMessage()
        {
            while (await _reviceMsgList.Reader.WaitToReadAsync())
            {
                if (_reviceMsgList.Reader.TryRead(out var message))
                {
                    if (_Clients.ContainsKey(message.SessionId))
                    {
                        _Clients[message.SessionId].ProcessPacket(message.ReviceData);
                    }
                }
            }
        }

        public static void AddPacket(string sessionId, string reviceData)
        {
            var clientPacket = new RecvicePacket();
            clientPacket.SessionId = sessionId;
            clientPacket.ReviceData = reviceData;
            _reviceMsgList.Writer.TryWrite(clientPacket);
        }

        public static void AddClient(string sessionId, RobotClient objClient)
        {
            _Clients.TryAdd(sessionId, objClient);
        }

        public static void DelClient(RobotClient objClient)
        {
            //_Clients.Remove(objClient);
        }

        public static void Run()
        {
            dwRunTick = HUtil32.GetTickCount();
            var boProcessLimit = false;
            var clientList = _Clients.Values.ToList();
            for (var i = g_nPosition; i < _Clients.Count; i++)
            {
                clientList[i].Run();
                if (((HUtil32.GetTickCount() - dwRunTick) > 20))
                {
                    g_nPosition = i;
                    boProcessLimit = true;
                    break;
                }
            }
            if (!boProcessLimit)
            {
                g_nPosition = 0;
            }
            g_dwProcessTimeMin = HUtil32.GetTickCount() - dwRunTick;
            if (g_dwProcessTimeMin > g_dwProcessTimeMax)
            {
                g_dwProcessTimeMax = g_dwProcessTimeMin;
            }
        }
    }

    public struct RecvicePacket
    {
        public string SessionId;
        public string ReviceData;
    }
}