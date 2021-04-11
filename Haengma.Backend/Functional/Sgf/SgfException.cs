using System;

namespace Haengma.Backend.Functional.Sgf
{
    public class SgfException : Exception
    {
        public SgfException(string? message) : base(message)
        {
        }
    }
}
