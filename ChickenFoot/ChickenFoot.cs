using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ChickenFoot
{
    public static class ChickenFoot
    {
        public static void Main()
        {
            Player[] players = new Player[6];
            players[0] = new GreedyPlayer(0, "0", 6);
            for (int id = 1; id < 6; id++)
            {
                players[id] = new RandomPlayer((uint)id, id.ToString(), 6);
            }

            for (int n = 0; n < 10; n++)
            {
                Game game = new Game(players);
                List<uint> scores = game.Play();

                System.Console.Clear();
                for (int id = 0; id < 6; id++)
                {
                    System.Console.WriteLine(string.Format("{0}:\t{1}", players[id].Name, scores[id]));
                }

                System.Console.In.ReadLine();
            }
        }
    }
}
