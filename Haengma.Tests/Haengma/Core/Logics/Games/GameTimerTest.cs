using Haengma.Core.Logics.Games;
using Xunit;
using static Xunit.Assert;
using static Haengma.Core.Logics.Games.GameTimer;
using System;

namespace Haengma.Tests.Haengma.Core.Logics.Games
{
    public class GameTimerTest
    {
        private static readonly DateTime StartedTime = new(2021, 6, 10, 22, 46, 0);

        [Fact]
        public void MainTime_ThatIsNotStarted_WillNotTick()
        {
            var mainTime = new MainTime(60);
            var actual = mainTime.Tick(DateTime.Now);
            Equal(mainTime, actual);
        }

        [Fact]
        public void ByoYomi_ThatIsNotStarted_WillNotTick()
        {
            var byoYomi = new ByoYomi(3, 10, 10);
            var actual = byoYomi.Tick(StartedTime);
            Equal(byoYomi, actual);
        }

        [Fact]
        public void MainTime_Started_WillTickElapsedSeconds()
        {
            var mainTime = new MainTime(60).Start(StartedTime);

            var actualMainTime = (MainTime)mainTime.Tick(StartedTime.AddSeconds(30));

            Equal(30, actualMainTime.SecondsLeft);
            Equal(StartedTime, actualMainTime.Started);
        }

        [Fact]
        public void ByoYomi_Started_WillTickElapsedSeconds()
        {
            var byoYomi = new ByoYomi(3, 10, 10).Start(StartedTime);
            var actualByoYomi = (ByoYomi)byoYomi.Tick(StartedTime.AddSeconds(15));

            Equal(2, actualByoYomi.Period);
            Equal(5, actualByoYomi.CurrentSeconds);
            Equal(10, actualByoYomi.TotalSeconds);
        }

        [Fact]
        public void MainTime_Started_TimeHasEnded_WhenElapsedSecondsExceedsSecondsLeft()
        {
            var mainTime = new MainTime(60).Start(StartedTime).Tick(StartedTime.AddSeconds(60));
            True(mainTime.HasTimeEnded());
        }

        [Fact]
        public void ByoYomi_Started_TimeHasEnded_WhenAllPeriodsHasRunOut()
        {
            var byoYomi = new ByoYomi(3, 10, 10).Start(StartedTime).Tick(StartedTime.AddSeconds(3 * 10));
            True(byoYomi.HasTimeEnded());
        }

        [Fact]
        public void MainTime_Start_StartedNotNull()
        {
            var mainTime = new MainTime(60).Start(StartedTime);
            Equal(StartedTime, mainTime.Started);
            True(mainTime.HasStarted());
        }

        [Fact]
        public void ByoYomi_Start_StartedNotNull()
        {
            var byoYomi = new ByoYomi(3, 10, 10).Start(StartedTime);
            Equal(StartedTime, byoYomi.Started);
            True(byoYomi.HasStarted());
        }

        [Fact]
        public void MainTime_Started_Stop_StartedNull()
        {
            var mainTime = new MainTime(60).Start(StartedTime).Stop();
            Null(mainTime.Started);
            False(mainTime.HasStarted());
        }

        [Fact]
        public void ByoYomi_Started_Stop_StartedNull()
        {
            var byoYomi = new ByoYomi(3, 10, 10).Start(StartedTime).Stop();
            Null(byoYomi.Started);
            False(byoYomi.HasStarted());
        }
    }
}
