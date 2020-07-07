using System.IO;

namespace Haengma.SGF
{
    public class SgfWriter : ISgfWriter
    {
        public void Write(TextWriter writer, SgfCollection collection)
        {
            foreach (var tree in collection)
            {
                WriteGameTree(writer, tree);
            }
        }

        private void WriteGameTree(TextWriter writer, SgfGameTree tree)
        {
            writer.Write('(');
            foreach (var node in tree)
            {
                writer.Write(';');

                foreach (var property in node)
                {
                    writer.Write(property.Identifier);
                    foreach (var value in property)
                    {
                        writer.Write('[');
                        writer.Write(value);
                        writer.Write(']');
                    }
                }
            }
            
            foreach (var t in tree.GameTrees)
            {
                WriteGameTree(writer, t);
            }

            writer.Write(')');
        }
    }
}
