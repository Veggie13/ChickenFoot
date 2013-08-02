using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChickenFoot
{
    public class GreedyPlayer : Player
    {
        public GreedyPlayer(uint id, string name, uint count)
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

            int selection = 0;
            for (int n = 1; n < validTiles.Count; n++)
            {
                if (validTiles[n].TotalValue > validTiles[selection].TotalValue)
                    selection = n;
            }

            if (available.Contains(validTiles[selection].LowValue) &&
                available.Contains(validTiles[selection].HighValue))
            {
                Random rand = new Random();
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

            int selection = 0;
            for (int n = 1; n < validTiles.Count; n++)
            {
                if (validTiles[n].TotalValue > validTiles[selection].TotalValue)
                    selection = n;
            }

            return validTiles[selection];
        }

        public override void ObservePlay(uint playerId, uint playedOn, Domino played)
        {
            System.Console.Write(string.Format("{0}:\t{1} on {2}",
                playerId, played.OtherValue(playedOn), playedOn));
            System.Console.In.ReadLine();/**/
        }
    }
}
