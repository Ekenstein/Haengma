using System;
using System.Collections.Generic;

namespace GTP
{
    public sealed class StoneStatus : IEquatable<StoneStatus?>
    {
        public static readonly StoneStatus Alive = new StoneStatus("alive");
        public static readonly StoneStatus Seki = new StoneStatus("seki");
        public static readonly StoneStatus Dead = new StoneStatus("dead");

        private readonly string _status;

        private StoneStatus(string status)
        {
            _status = status;
        }

        public override string ToString() => _status;

        public override bool Equals(object? obj)
        {
            return Equals(obj as StoneStatus);
        }

        public bool Equals(StoneStatus? other)
        {
            return other != null &&
                   _status == other._status;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_status);
        }

        public static bool operator ==(StoneStatus? left, StoneStatus? right)
        {
            return EqualityComparer<StoneStatus>.Default.Equals(left, right);
        }

        public static bool operator !=(StoneStatus? left, StoneStatus? right)
        {
            return !(left == right);
        }
    }
}
