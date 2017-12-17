using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Descent_Server
{
    public class Player
    {
        // TOOD: Gör så att klassen håller reda på vilka chunks som är synliga

        // References
        readonly NetConnection _controllingClient;

        List<Character> _ownedCharacters = new List<Character>();


        // Constructor
        public Player(NetConnection inOwnerClient)
        {
            _controllingClient = inOwnerClient;

            AddOwnedCharacter(new Character(World.GetChunk(Vector2DI.Zero).data.GetTile(0,0)));
        }


        // External
        void AddOwnedCharacter(Character inCharacter)
        {
            _ownedCharacters.Add(inCharacter);
        }
    }
}
