using System;
using System.Collections.Generic;
using System.Text;

namespace GTP.Commands
{
    public class TimeLeft : GtpCommand
    {
        /// <summary>
        /// Color for which the information applies.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Number of seconds remaining.
        /// </summary>
        public int Time { get; }

        /// <summary>
        /// Number of stones remaining.
        /// </summary>
        public int Stones { get; }

        public TimeLeft(int? id, Color color, int time, int stones) : base(id, "time_left", new [] { color.ToString(), time.ToString(), stones.ToString() })
        {
            Color = color;
            Time = time;
            Stones = stones;
        }

        public override string ToString() => "Time left";
    }
}
