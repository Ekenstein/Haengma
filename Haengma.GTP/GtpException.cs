using System;

namespace GTP
{
    public class GtpException : Exception
    {
        public GtpException(string message) : base(message) { }
    }
}
