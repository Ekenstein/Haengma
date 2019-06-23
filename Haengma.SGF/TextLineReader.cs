using System;
using System.IO;
using System.Linq;

namespace Haengma.SGF
{
    public class TextLineReader : TextReader
    {
        private static readonly char[] Linebreaks = new char[] { '\n', '\r' };
        private readonly TextReader _reader;

        public int Line { get; private set; }
        public int Column { get; private set; }

        public bool EOF { get; private set; }

        public TextLineReader(TextReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public override void Close() => _reader.Close();
        public override object InitializeLifetimeService() => _reader.InitializeLifetimeService();

        public override int Peek() => _reader.Peek();

        public override int Read()
        {
            var result = _reader.Read();
            switch (result)
            {
                case -1:
                    EOF = true;
                    break;
                case var _ when Linebreaks.Contains((char)result):
                    Line++;
                    Column = 0;
                    break;
                default:
                    Column++;
                    break;
            }
            
            return result;
        }
    }
}
