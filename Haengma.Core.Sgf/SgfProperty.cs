using Haengma.Core.Utils;
using Pidgin;
using System.Collections.Generic;

namespace Haengma.Core.Sgf
{
    public abstract record SgfProperty
    {
        public record B(Move Move) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record W(Move Move) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record C(string Comment) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record PB(string Name) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record PW(string Name) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record AB(Set<Point> Stones) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record AW(Set<Point> Stones) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record SZ(int Size) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record HA(int Handicap) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record MN(int MoveNumber) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record KM(double Komi) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record PL(SgfColor Color) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record AP((string name, string version) Application) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record BR(string Rank) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record WR(string Rank) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record Unknown(string Identifier, IReadOnlyList<string> Values) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record OT(string Overtime) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record RE(string Result) : SgfProperty()
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        internal abstract T Accept<T>(ISgfPropertyVisitor<T> visitor);
        internal void Accept(ISgfPropertyVisitor<Unit> visitor) => Accept<Unit>(visitor);
    }
}
