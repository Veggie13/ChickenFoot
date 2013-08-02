using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChickenFoot
{
    public sealed class Game
    {
        #region Static Helpers
        private static List<Domino> GenerateTiles()
        {
            List<Domino> tiles = new List<Domino>();
            for (uint low = 0; low <= 12; low++)
            {
                for (uint high = low; high <= 12; high++)
                {
                    tiles.Add(Domino.Get(low, high));
                }
            }
            return tiles;
        }

        private static void ShuffleTiles(List<Domino> tiles)
        {
            Random rand = new Random();
            for (int i = tiles.Count - 1; i > 0; i--)
            {
                int n = rand.Next(i + 1);
                Domino temp = tiles[i];
                tiles[i] = tiles[n];
                tiles[n] = temp;
            }
        }
        #endregion

        #region Private Members
        private List<Player> _players = new List<Player>();
        private List<Domino> _tiles;
        private List<uint> _valuesRemaining = new List<uint>(
            (new uint[13] { 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }).AsEnumerable());
        private List<uint> _availablePlays = new List<uint>();
        private uint _currPlayerId;
        private uint _turnCount = 0;
        #endregion
        
        #region Private Methods
        private Player CurrPlayer
        {
            get
            {
                return _players[(int)_currPlayerId];
            }
        }

        private bool PlaceTile(uint playedOn, Domino tilePlayed)
        {
            CurrPlayer.Remove(tilePlayed);

            _availablePlays.Remove(playedOn);
            _availablePlays.Add(tilePlayed.OtherValue(playedOn));

            
            NotifyPlayers(playedOn, tilePlayed);

            if (tilePlayed.HasBeenPlayed)
            {
                System.Console.WriteLine(
                    string.Format("*** Tile {0}:{1} played again ***",
                    tilePlayed.LowValue, tilePlayed.HighValue));
                System.Console.In.ReadLine();
            }
            tilePlayed.HasBeenPlayed = true;

            return (CurrPlayer.DominosRemaining == 0);
        }

        private Domino PickUpTile()
        {
            if (_tiles.Count == 0)
                return null;

            Domino tile = _tiles[0];
            _tiles.Remove(tile);
            return tile;
        }

        private void NotifyPlayers(uint playedOn, Domino tilePlayed)
        {
            foreach (Player player in _players)
            {
                player.ObservePlay(_currPlayerId, playedOn, tilePlayed);
            }
        }

        private void DealTiles()
        {
            for (int i = 0; i < 6; i++)
            {
                foreach (Player player in _players)
                {
                    player.PickUp(PickUpTile());
                }
            }
        }

        private bool FindStartingTile(ref uint startTile, ref uint startPlayerId)
        {
            IEnumerator<uint> it = _valuesRemaining.GetEnumerator();
            for (; it.MoveNext(); )
            {
                foreach (Player player in _players)
                {
                    if (player.Contains(Domino.Get(it.Current, it.Current)))
                    {
                        startTile = it.Current;
                        _valuesRemaining.Remove(startTile);
                        startPlayerId = player.ID;
                        return true;
                    }
                }
            }

            return false;
        }

        private void NextPlayer()
        {
            _currPlayerId = (_currPlayerId + 1) % (uint)_players.Count;
            _turnCount++;
        }

        private void PrevPlayer()
        {
            _currPlayerId = (_currPlayerId + (uint)_players.Count - 1) % (uint)_players.Count;
            _turnCount--;
        }

        private bool ConductChickenFoot(uint startTile)
        {
            _availablePlays.Add(startTile);
            _availablePlays.Add(startTile);

            uint tileCount = 0, failCount = 0;
            while (tileCount < 3)
            {
                NextPlayer();

                Domino playedTile = CurrPlayer.TakeBranchTurn(startTile);
                if (playedTile != null)
                {
                    tileCount++;
                    if (PlaceTile(startTile, playedTile))
                        return true;
                }
                else
                {
                    if (_tiles.Count == 0 && ++failCount == _players.Count)
                        return false;

                    CurrPlayer.PickUp(PickUpTile());
                    playedTile = CurrPlayer.TakeBranchTurn(startTile);
                    if (playedTile != null)
                    {
                        tileCount++;
                        if (PlaceTile(startTile, playedTile))
                            return true;
                    }
                    else
                    {
                        System.Console.WriteLine(string.Format("{0}:\t Did not play", _currPlayerId));
                    }
                }
            }

            return true;
        }

        private List<uint> ComputeScores()
        {
            List<uint> scores = new List<uint>();
            foreach (Player player in _players)
            {
                scores.Add(player.Score);
            }

            return scores;
        }
        #endregion

        public Game(IList<Player> players)
        {
            _players.AddRange(players);
        }

        public List<uint> Play()
        {
            List<uint> scores = ComputeScores();

            while (_valuesRemaining.Count > 0)
            {
                #region Initialize Tiles
                _availablePlays.RemoveRange(0, _availablePlays.Count);
                foreach (Player player in _players)
                {
                    player.ClearHand();
                }
                _tiles = GenerateTiles();
                ShuffleTiles(_tiles);
                DealTiles();
                #endregion

                #region Begin Round
                Domino.ResetAll();
                System.Console.WriteLine("New Round");

                uint startTile = 100, startPlayer = 100;
                _currPlayerId = 0;
                while (!FindStartingTile(ref startTile, ref startPlayer))
                {
                    CurrPlayer.PickUp(PickUpTile());
                    if (FindStartingTile(ref startTile, ref startPlayer))
                        break;

                    NextPlayer();
                }

                _currPlayerId = startPlayer;
                _availablePlays.Add(startTile);

                Domino firstTile = Domino.Get(startTile, startTile);
                PlaceTile(startTile, firstTile);

                ConductChickenFoot(startTile);
                ConductChickenFoot(startTile);
                #endregion

                #region Main Game Loop
                while (CurrPlayer.DominosRemaining > 0)
                {
                    NextPlayer();

                    uint playedOn = 100;
                    Domino playedTile = CurrPlayer.TakeTurn(_availablePlays, ref playedOn);
                    if (playedTile != null)
                    {
                        if (PlaceTile(playedOn, playedTile))
                            continue;
                        if (playedTile.LowValue == playedTile.HighValue && !ConductChickenFoot(playedOn))
                            break;
                    }
                    else
                    {
                        CurrPlayer.PickUp(PickUpTile());
                        playedTile = CurrPlayer.TakeTurn(_availablePlays, ref playedOn);
                        if (playedTile != null)
                        {
                            if (PlaceTile(playedOn, playedTile))
                                continue;
                            if (playedTile.LowValue == playedTile.HighValue && !ConductChickenFoot(playedOn))
                                break;
                        }
                        else
                        {
                            System.Console.WriteLine(string.Format("{0}:\t Did not play", _currPlayerId));
                        }
                    }
                }
                #endregion

                #region Score Sheet
                List<uint> roundScores = ComputeScores();
                for (int playerId = 0; playerId < _players.Count; playerId++)
                {
                    scores[playerId] += roundScores[playerId];
                }
                #endregion
            }

            return scores;
        }
    }
}
