using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lidgren.Network;

public class NetworkManager : MonoBehaviour
{
    // Private
    static NetClient _client;




    // Start, Update...
    void Start()
    {
        (_client = new NetClient(new NetPeerConfiguration("Descent_Server"))).Start();
        _client.Connect(host: "127.0.0.1", port: 12345);
    }

    void Update()
    {
        ProcessMessages();
    }




    // External
    public static void Send(IInPackable inPacket, NetDeliveryMethod inDeliveryMethod)
    {
        var newMessage = _client.CreateMessage();
        inPacket.PackInto(newMessage);
        _client.SendMessage(newMessage, inDeliveryMethod);
    }




    // Internal
    void ProcessMessages()
    {
        NetIncomingMessage message;
        while ((message = _client.ReadMessage()) != null)
        {
            switch (message.MessageType)
            {
                case NetIncomingMessageType.Data:
                    ProcessDataMessage(message);
                    break;

                case NetIncomingMessageType.StatusChanged:
                    OnStatusChange((NetConnectionStatus)message.ReadByte());
                    Debug.Log(message.ReadString());
                    break;

                case NetIncomingMessageType.ErrorMessage:
                    Debug.Log(message.ReadString());
                    break;

                case NetIncomingMessageType.WarningMessage:
                    Debug.Log(message.ReadString());
                    break;
            }
            _client.Recycle(message);
        }
    }

    void OnStatusChange(NetConnectionStatus inNewStatus)
    {
        if (inNewStatus == NetConnectionStatus.Connected)
        {
            Debug.Log(_client.UniqueIdentifier);
        }
    }

    void ProcessDataMessage(NetIncomingMessage inMsg)
    {
        EDataPacketTypes packetType = (EDataPacketTypes)inMsg.ReadByte();
        switch (packetType)
        {
            case EDataPacketTypes.WorldData:
                FindObjectOfType<World>().UnpackFrom(inMsg);
                break;

            case EDataPacketTypes.ChunkData:
                break;

            case EDataPacketTypes.TerrainData:
                Terrain.dataManager.UnpackFrom(inMsg);
                break;

            default:
                break;
        }
    }
}