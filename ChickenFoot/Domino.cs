using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChickenFoot
{
    using UIntPair = KeyValuePair<uint, uint>;
    using DominoMap = Dictionary<KeyValuePair<uint, uint>, Domino>;

    public sealed class Domino
    {
        private static DominoMap s_map = new DominoMap();

        private uint _lowValue;
        private uint _highValue;

        private bool _played = false;

        public static Domino Get(uint low, uint high)
        {
            UIntPair key = new UIntPair(low, high);
            Domino retVal = null;

            if (s_map.TryGetValue(key, out retVal))
                return retVal;

            return new Domino(low, high);
        }

        public static void ResetAll()
        {
            foreach (Domino dom in s_map.Values)
            {
                dom.HasBeenPlayed = false;
            }
        }

        private Domino(uint low, uint high)
        {
            if (low > high)
            {
                _lowValue = high;
                _highValue = low;
            }
            else
            {
                _lowValue = low;
                _highValue = high;
            }

            s_map.Add(new UIntPair(_lowValue, _highValue), this);
        }

        public uint LowValue
        {
            get { return _lowValue; }
        }

        public uint HighValue
        {
            get { return _highValue; }
        }

        public uint TotalValue
        {
            get
            {
                if (_highValue == 0)
                    return 50;
                return _lowValue + _highValue;
            }
        }

        public bool HasBeenPlayed
        {
            get { return _played; }
            set { _played = value; }
        }

        public bool Contains(uint value)
        {
            return _lowValue == value || _highValue == value;
        }

        public uint OtherValue(uint value)
        {
            if (_lowValue == value)
                return _highValue;
            else if (_highValue == value)
                return _lowValue;

            throw new InvalidOperationException();
        }
    }
}
