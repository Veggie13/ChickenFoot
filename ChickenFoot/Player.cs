using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChickenFoot
{
    public interface IPlayer
    {
        string Name
        {
            get;
        }

        uint ID
        {
            get;
        }

        uint DominosRemaining
        {
            get;
        }

        bool Contains(Domino dom);
    }

    public abstract class Player : IPlayer
    {
        private uint _id;
        private uint _playerCount;
        private string _name;
        private List<Domino> _hand = new List<Domino>();

        public Player(uint id, string name, uint count)
        {
            _name = name;
            _id = id;
            _playerCount = count;
        }

        public string Name
        {
            get { return _name; }
        }

        public uint ID
        {
            get { return _id; }
        }

        public uint DominosRemaining
        {
            get { return (uint)_hand.Count; }
        }

        public bool Contains(Domino dom)
        {
            return _hand.Contains(dom);
        }

        protected IEnumerable<Domino> Hand
        {
            get { return _hand; }
        }

        protected uint NumPlayers
        {
            get { return _playerCount; }
        }

        public void PickUp(Domino dom)
        {
            if (dom != null)
                _hand.Add(dom);
        }

        public void Remove(Domino dom)
        {
            _hand.Remove(dom);
        }

        public void ClearHand()
        {
            _hand.RemoveRange(0, _hand.Count);
        }

        public uint Score
        {
            get
            {
                uint total = 0;

                foreach (Domino dom in _hand)
                    total += dom.TotalValue;

                return total;
            }
        }

        public virtual void ObservePlay(uint playerId, uint playedOn, Domino played)
        {
            // Do nothing
        }

        public abstract Domino TakeTurn(IEnumerable<uint> available, ref uint playOn);
        public abstract Domino TakeBranchTurn(uint branch);
    }
}
