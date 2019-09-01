using System.IO;
using Monadicsh;

namespace Haengma.SGF
{
    public delegate Maybe<string> ValueParser(TextReader reader);

    public class SgfReader : ISgfReader
    {
        public Maybe<SgfCollection> Parse(TextReader reader)
        {
            var collection = new SgfCollection();

            Maybe<SgfGameTree> next;
            while ((next = ParseGameTree(reader, true)).IsJust)
            {
                collection.GameTrees.Add(next.Value);
            }

            return collection;
        }

        private Maybe<SgfGameTree> ParseGameTree(TextReader reader, bool root)
        {
            reader.SkipWhitespace();
            var opening = reader.Read(c => c == '(');
            if (opening.IsNothing)
            {
                return Maybe<SgfGameTree>.Nothing;
            }

            var gameTree = new SgfGameTree();

            var rootNode = root;

            Maybe<SgfNode> nextNode;
            while ((nextNode = ParseNode(reader)).IsJust)
            {
                gameTree.Sequence.Add(nextNode.Value);
                rootNode = false;
            }

            Maybe<SgfGameTree> nextGameTree;
            while ((nextGameTree = ParseGameTree(reader, false)).IsJust)
            {
                gameTree.GameTrees.Add(nextGameTree.Value);
            }

            reader.SkipWhitespace();
            var ending = reader.Read(c => c == ')');
            if (ending.IsNothing)
            {
                return Maybe<SgfGameTree>.Nothing;
            }

            return gameTree;
        }

        private Maybe<SgfNode> ParseNode(TextReader reader)
        {
            reader.SkipWhitespace();
            var start = reader.Read(c => c == ';');
            if (start.IsNothing)
            {
                return Maybe<SgfNode>.Nothing;
            }

            var node = new SgfNode();

            Maybe<SgfProperty> next;
            while ((next = ParseProperty(reader)).IsJust)
            {
                node.Properties.Add(next.Value);
            }

            return node;
        }

        private Maybe<SgfProperty> ParseProperty(TextReader reader)
        {
            reader.SkipWhitespace();

            var identifier = reader.ReadString(char.IsUpper);
            if (identifier.IsNothing)
            {
                return Maybe<SgfProperty>.Nothing;
            }

            var property = new SgfProperty(identifier.Value);

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

            var value = PropertyValueParsers.ParseText(reader, false);
            if (value.IsNothing)
            {
                return Maybe<string>.Nothing;
            }

            var end = reader.Read(c => c == ']');
            if (end.IsNothing)
            {
                return Maybe<string>.Nothing;
            }

            return value.Value.ToString();
        }
    }
}
