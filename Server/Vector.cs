using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Descent_Server
{
    public struct Vector2DI : IPackable
    {
        // Data
        public int x, y;

        public Vector2DI Zero { get { return new Vector2DI(0, 0); } }



        // Constructor
        public Vector2DI(int inX, int inY)
        {
            x = inX;
            y = inY;
        }



        // Networking
        public int GetPacketSize()
        {
            
            return NetUtility.BitsToHoldUInt((uint)Math.Abs(x)) + NetUtility.BitsToHoldUInt((uint)Math.Abs(y)) + 16;
        }

        public void PackInto(NetOutgoingMessage inMsg)
        {
            inMsg.WriteVariableUInt32((uint)Math.Abs(x));
            inMsg.WriteVariableUInt32((uint)Math.Abs(y));

            if (x < 0) inMsg.Write(false); else inMsg.Write(true);
            if (y < 0) inMsg.Write(false); else inMsg.Write(true);
        }

        public void UnpackFrom(NetIncomingMessage inMsg)
        {
            x = (int)inMsg.ReadUInt32();
            y = (int)inMsg.ReadUInt32();

            if (!inMsg.ReadBoolean()) x = -x;
            if (!inMsg.ReadBoolean()) y = -y;
        }
    }
}
