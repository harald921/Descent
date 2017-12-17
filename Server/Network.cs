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
        // Data
        const int _port = 12345;

        // References
        static NetServer _server;

        // Events
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

        public static void Send(IInPackable inPacket, EDataPacketTypes inMsgType, NetConnection inTargetConnection, NetDeliveryMethod inDeliveryMethod)
        {
            var newMsg = _server.CreateMessage(inPacket.GetPacketSize());
            newMsg.Write((byte)inMsgType);
            inPacket.PackInto(newMsg);
            _server.SendMessage(newMsg, inTargetConnection, inDeliveryMethod);
        }
        



        // Internal
        void OnClientStatusChanged(NetConnectionStatus inNewStatus, NetIncomingMessage inMsg)
        {
            if (inNewStatus == NetConnectionStatus.Connected)
            {
                OnPlayerJoin?.Invoke(inMsg.SenderConnection);
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
    }
}
