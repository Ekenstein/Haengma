using System;
using static Haengma.Core.Logics.Games.GameTimer;

namespace Haengma.Core.Logics.Games
{
    public abstract record GameTimer(DateTime? Started)
    {
        public sealed record MainTime(int SecondsLeft, DateTime? Started = null) : GameTimer(Started);
        public sealed record ByoYomi(int Period, int CurrentSeconds, int TotalSeconds, DateTime? Started = null) : GameTimer(Started);
    }

    public static class GameTimerExtensions
    {
        private static ByoYomi Reset(this ByoYomi byoYomi) => byoYomi with
        {
            CurrentSeconds = byoYomi.TotalSeconds
        };

        private static int ElapsedSeconds(this GameTimer timer, DateTime now)
        {
            var started = timer.Started ?? now;
            var timespan = now - started;
            return (int)timespan.TotalSeconds;
        }

        public static GameTimer Start(this GameTimer timer, DateTime now) => timer switch
        {
            ByoYomi byoYomi => byoYomi.Reset() with { Started = now },
            _ => timer with { Started = now }
        };

        public static bool HasStarted(this GameTimer timer) => timer.Started != null;

        public static GameTimer Stop(this GameTimer timer) => timer with { Started = null };

        public static bool HasTimeEnded(this GameTimer timer) => timer switch
        {
            ByoYomi byoYomi => byoYomi.Period <= 0,
            MainTime mainTime => mainTime.SecondsLeft <= 0,
            _ => throw new ArgumentOutOfRangeException(nameof(timer), timer, "Couldn't recognize the given timer.")
        };

        public static GameTimer Tick(this GameTimer timer, DateTime now)
        {
            var elapsedSeconds = timer.ElapsedSeconds(now);
            return timer switch
            {
                MainTime main => main with { SecondsLeft = Math.Max(0, main.SecondsLeft - elapsedSeconds) },
                ByoYomi byoYomi => TickByoYomi(byoYomi, elapsedSeconds),
                _ => throw new ArgumentOutOfRangeException(nameof(timer), timer, "Couldn't recognize the given timer.")
            };
        }

        private static ByoYomi TickByoYomi(ByoYomi byoYomi, int seconds)
        {
            if (byoYomi.Period == 0)
            {
                return byoYomi with { CurrentSeconds = 0 };
            }

            var currentSeconds = byoYomi.CurrentSeconds - seconds;
            if (currentSeconds >= 1)
            {
                return byoYomi with
                {
                    CurrentSeconds = currentSeconds
                };
            }

            if (currentSeconds == 0)
            {
                return byoYomi with
                {
                    CurrentSeconds = byoYomi.TotalSeconds,
                    Period = Math.Max(0, byoYomi.Period - 1)
                };
            }

            return TickByoYomi(byoYomi with
            {
                Period = Math.Max(0, byoYomi.Period - 1),
                CurrentSeconds = byoYomi.TotalSeconds
            }, Math.Abs(currentSeconds));
        }
    }
}
