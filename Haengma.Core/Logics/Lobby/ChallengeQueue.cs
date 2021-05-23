using Haengma.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.Core.Logics.Lobby
{
    public class ChallengeQueue
    {
        private readonly LinkedList<GameChallenge> _queue = new();

        public bool Enqueue(GameChallenge t)
        {
            if (_queue.Any(x => x.Challenger == t.Challenger))
            {
                return false;
            }

            _queue.AddLast(t);
            return true;
        }

        public GameChallenge? Dequeue()
        {
            var first = _queue.First?.Value;

            if (first != null)
            {
                _queue.RemoveFirst();
            }

            return first;
        }

        public GameChallenge? Peek() => _queue.First?.Value;

        public bool Remove(GameChallenge t) => _queue.Remove(t);

        public bool RemoveByUserId(UserId userId)
        {
            var item = _queue.SingleOrDefault(x => x.Challenger == userId);
            if (item != null)
            {
                return Remove(item);
            }

            return false;
        }

        public IReadOnlyList<GameChallenge> All() => _queue.ToArray();
    }
}
