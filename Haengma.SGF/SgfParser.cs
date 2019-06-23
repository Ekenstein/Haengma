using System.IO;
using System.Linq;
using System.Text;
using Monadicsh;
using Monadicsh.Extensions;

namespace Haengma.SGF
{
    public class SgfParser : ISgfParser
    {
        public SgfParserConfig Configuration { get; } = new SgfParserConfig();

        public Collection Parse(TextReader reader)
        {
            var lineReader = new TextLineReader(reader);
            var collection = new Collection();

            Maybe<GameTree> next;
            while ((next = ParseGameTree(lineReader)).IsJust)
            {
                collection.GameTrees.Add(next.Value);
            }

            return collection;
        }

        public Maybe<GameTree> ParseGameTree(TextReader reader)
        {
            reader.SkipWhitespace();
            var opening = reader.Read(c => c == '(');
            if (opening.IsNothing)
            {
                return Maybe<GameTree>.Nothing;
            }

            var gameTree = new GameTree();

            Maybe<Node> nextNode;
            while ((nextNode = ParseNode(reader)).IsJust)
            {
                gameTree.Sequence.Add(nextNode.Value);
            }

            Maybe<GameTree> nextGameTree;
            while ((nextGameTree = ParseGameTree(reader)).IsJust)
            {
                gameTree.GameTrees.Add(nextGameTree.Value);
            }

            reader.SkipWhitespace();
            var ending = reader.Read(c => c == ')');
            if (ending.IsNothing)
            {
                return Maybe<GameTree>.Nothing;
            }

            return gameTree;
        }

        public Maybe<Node> ParseNode(TextReader reader)
        {
            reader.SkipWhitespace();
            var start = reader.Read(c => c == ';');
            if (start.IsNothing)
            {
                return Maybe<Node>.Nothing;
            }

            var node = new Node();

            Maybe<Property> next;
            while ((next = ParseProperty(reader)).IsJust)
            {
                node.Properties.Add(next.Value);
            }

            return node;
        }

        private Maybe<Property> ParseProperty(TextReader reader)
        {
            reader.SkipWhitespace();

            var identifier = reader.ReadString(char.IsUpper);
            if (identifier.IsNothing)
            {
                return Maybe<Property>.Nothing;
            }

            var property = new Property(identifier.Value);

            Maybe<string> nextValue;
            while ((nextValue = ParsePropertyValue(reader)).IsJust)
            {
                property.Values.Add(nextValue.Value);
            }

            return property;
        }

        private Maybe<string> ParsePropertyValue(TextReader reader)
        {
            reader.SkipWhitespace();
            var start = reader.Read(c => c == '[');
            if (start.IsNothing)
            {
                return Maybe<string>.Nothing;
            }

            var value = new StringBuilder();
            Maybe<char> next;

            while ((next = NormalChar(reader).Or(() => EscapedChar(reader))).IsJust)
            {
                value.Append(next.Value);
            }

            var end = reader.Read(c => c == ']');
            if (end.IsNothing)
            {
                return Maybe<string>.Nothing;
            }

            return value.ToString();
        }

        private Maybe<char> NormalChar(TextReader reader)
        {
            return reader.Read(c => !NeedsEscape.Contains(c));
        }

        private Maybe<char> EscapedChar(TextReader reader)
        {
            if (reader.Read(EscapeChar).IsNothing)
            {
                return Maybe<char>.Nothing;
            }

            reader.Skip(Linebreaks.Contains);
            return reader
                .Read(NeedsEscape.Contains)
                .Or(() => NormalChar(reader));
        }

        private static bool EscapeChar(char c) => c == '\\';

        private static readonly char[] NeedsEscape = new [] { ']', '\\' };
        private static readonly char[] Linebreaks = new [] { '\n', '\r' };
    }
}
