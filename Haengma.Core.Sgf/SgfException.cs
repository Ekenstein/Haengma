using System;

namespace Haengma.Core.Sgf
{
    public class SgfException : Exception
    {
        public SgfException(string? message) : base(message)
        {
        }
    }
}
