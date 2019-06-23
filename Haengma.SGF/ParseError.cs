using Monadicsh;
using System;
using System.Collections.Generic;
using System.Text;

namespace Haengma.SGF
{
    public class ParseError
    {
        public int Line { get; }
        public int Column { get; }
        public IReadOnlyList<Error> Errors { get; }
    }
}
