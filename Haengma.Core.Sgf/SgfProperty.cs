using Pidgin;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Haengma.Tests")]
namespace Haengma.Core.Sgf
{
    public enum SgfPropertyType
    {
        Move,
        Setup,
        NodeAnnotation,
        Root,
        Timing,
        GameInfo,
        Unknown
    }

    public abstract record SgfProperty(SgfPropertyType Type)
    {
        public record B(Move Move) : SgfProperty(SgfPropertyType.Move)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record W(Move Move) : SgfProperty(SgfPropertyType.Move)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record C(string Comment) : SgfProperty(SgfPropertyType.NodeAnnotation)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record PB(string Name) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record PW(string Name) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record AB(IReadOnlySet<Point> Stones) : SgfProperty(SgfPropertyType.Setup)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record AW(IReadOnlySet<Point> Stones) : SgfProperty(SgfPropertyType.Setup)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record SZ(int Size) : SgfProperty(SgfPropertyType.Root)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record HA(int Handicap) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record MN(int MoveNumber) : SgfProperty(SgfPropertyType.Move)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record KM(double Komi) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record PL(SgfColor Color) : SgfProperty(SgfPropertyType.Setup)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record AP((string name, string version) Application) : SgfProperty(SgfPropertyType.Root)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record BR(string Rank) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record WR(string Rank) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record Unknown(string Identifier, IReadOnlyList<string> Values) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record OT(string Overtime) : SgfProperty(SgfPropertyType.Timing)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record RE(string Result) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record Emote(SgfColor Color, SgfEmote Message) : SgfProperty(SgfPropertyType.NodeAnnotation)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        internal abstract T Accept<T>(ISgfPropertyVisitor<T> visitor);
        internal void Accept(ISgfPropertyVisitor<Unit> visitor) => Accept<Unit>(visitor);
    }
}
