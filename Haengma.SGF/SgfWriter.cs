using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Haengma.SGF
{
    public class SgfWriter : ISgfWriter
    {
        public void Write(TextWriter writer, SgfCollection collection)
        {
            foreach (var tree in collection.GameTrees)
            {
                WriteGameTree(writer, tree);
            }
        }

        private void WriteGameTree(TextWriter writer, SgfGameTree tree)
        {
            writer.Write('(');
            foreach (var node in tree.Sequence)
            {
                writer.Write(';');

                foreach (var property in node.Properties)
                {
                    writer.Write(property.Identifier);
                    foreach (var value in property.Values)
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
