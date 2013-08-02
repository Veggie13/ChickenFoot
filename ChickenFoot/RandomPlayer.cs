using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChickenFoot
{
    public class RandomPlayer : Player
    {
        public RandomPlayer(uint id, string name, uint count)
            : base(id, name, count)
        {
        }

        public override Domino TakeTurn(IEnumerable<uint> available, ref uint playOn)
        {
            List<Domino> validTiles = new List<Domino>();
            foreach (Domino tile in Hand)
            {
                if (available.Contains(tile.LowValue) || available.Contains(tile.HighValue))
                {
                    validTiles.Add(tile);
                }
            }

            if (validTiles.Count == 0)
                return null;

            Random rand = new Random();
            int selection = rand.Next(validTiles.Count);
            if (available.Contains(validTiles[selection].LowValue) &&
                available.Contains(validTiles[selection].HighValue))
            {
                if (rand.Next(2) == 1)
                    playOn = validTiles[selection].HighValue;
                else
                    playOn = validTiles[selection].LowValue;
            }
            else if (available.Contains(validTiles[selection].LowValue))
                playOn = validTiles[selection].LowValue;
            else
                playOn = validTiles[selection].HighValue;

            return validTiles[selection];
        }

        public override Domino TakeBranchTurn(uint branch)
        {
            List<Domino> validTiles = new List<Domino>();
            foreach (Domino tile in Hand)
            {
                if (tile.LowValue == branch || tile.HighValue == branch)
                {
                    validTiles.Add(tile);
                }
            }

            if (validTiles.Count == 0)
                return null;

            Random rand = new Random();
            int selection = rand.Next(validTiles.Count);
            return validTiles[selection];
        }
    }
}
