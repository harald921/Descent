using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lidgren.Network;

namespace Descent_Server
{
    // Types
    public enum EDataPacketTypes
    {
        WorldData,
        ChunkData,
    }

    // Interface
    public interface IPackable : IInPackable, IUnPackable { }

    public interface IInPackable
    {
        int GetPacketSize();
        void PackInto(NetOutgoingMessage inMsg);
    }

    public interface IUnPackable
    {
        void UnpackFrom(NetIncomingMessage inMsg);
    }
}
