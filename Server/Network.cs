using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lidgren.Network;

namespace Descent_Server
{
    class Network
    {
        const int _port = 12345;

        static NetServer _server;


        public static event Action<NetConnection> OnPlayerJoin;

        // Constructor
        public Network(string inAppName)
        {
            _server = new NetServer(new NetPeerConfiguration(inAppName) { Port = _port });
            _server.Start();
        }


        // External
        public void ProcessMessages()
        {
            NetIncomingMessage message;
            while ((message = _server.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        ProcessDataMessage(message);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        OnClientStatusChanged((NetConnectionStatus)message.ReadByte(), message);
                        break;

                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(message.ReadString());
                        break;

                    case NetIncomingMessageType.WarningMessage:
                        Console.WriteLine(message.ReadString());
                        break;
                }
                _server.Recycle(message);
            }
        }

        // Internal
        public static void Send(IInPackable inPacket, EDataPacketTypes inMsgType, NetConnection inTargetConnection, NetDeliveryMethod inDeliveryMethod)
        {
            var newMsg = _server.CreateMessage(inPacket.GetPacketSize());
            newMsg.Write((byte)inMsgType);
            inPacket.PackInto(newMsg);
            _server.SendMessage(newMsg, inTargetConnection, inDeliveryMethod);
        }


        void OnClientStatusChanged(NetConnectionStatus inNewStatus, NetIncomingMessage inMsg)
        {
            if (inNewStatus == NetConnectionStatus.Connected)
            {
                OnPlayerJoin(inMsg.SenderConnection);
            }
        }

        void ProcessDataMessage(NetIncomingMessage inMessage)
        {
            EDataPacketTypes packetType = (EDataPacketTypes)inMessage.ReadByte();

            switch(packetType)
            {
                default:
                    Console.WriteLine("ERROR: " + "EPacketType of recieved message unknown!");
                    break;
            }
        }

        // public static int BitsNeeded(uint val)
        // {
        //     for (int i = 0; i < 32; i++)
        //         if (val >> i == 0)
        //             return i;
        // 
        //     return 32;
        // }
    }
}
