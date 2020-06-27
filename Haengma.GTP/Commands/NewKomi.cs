using System;
using System.Collections.Generic;
using System.Text;

namespace GTP.Commands
{
    /// <summary>
    /// Komi is changed.
    /// </summary>
    public class NewKomi : GtpCommand
    {
        public double Komi { get; }

        public NewKomi(int? id, double komi) : base(id, "new_komi", new [] { komi.ToString() })
        {
            Komi = komi;
        }

        public override string ToString() => $"New komi {Komi}";
    }
}
