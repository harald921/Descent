using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Descent_Server
{
    public class Character
    {
        // References
        Tile _currentTile;


        // Constructor
        public Character(Tile inCurrentTile)
        {
            _currentTile = inCurrentTile;
        }
    }
}
