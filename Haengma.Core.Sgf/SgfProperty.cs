using Haengma.Core.Utils;
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

        public record C(SgfText Comment) : SgfProperty(SgfPropertyType.NodeAnnotation)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record PB(SgfSimpleText Name) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record PW(SgfSimpleText Name) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record AB(NonEmptyReadOnlySet<SgfPoint> Stones) : SgfProperty(SgfPropertyType.Setup)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record AW(NonEmptyReadOnlySet<SgfPoint> Stones) : SgfProperty(SgfPropertyType.Setup)
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

        public record AP(SgfSimpleText Name, SgfSimpleText Version) : SgfProperty(SgfPropertyType.Root)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record BR(SgfSimpleText Rank) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record WR(SgfSimpleText Rank) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record Unknown(string Identifier, NonEmptyReadOnlyList<SgfText> Values) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record OT(SgfSimpleText Overtime) : SgfProperty(SgfPropertyType.Timing)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record RE(SgfSimpleText Result) : SgfProperty(SgfPropertyType.GameInfo)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        public record EM(SgfColor Color, SgfEmote Message) : SgfProperty(SgfPropertyType.NodeAnnotation)
        {
            internal override T Accept<T>(ISgfPropertyVisitor<T> visitor) => visitor.Accept(this);
        }

        internal abstract T Accept<T>(ISgfPropertyVisitor<T> visitor);
        internal void Accept(ISgfPropertyVisitor<Unit> visitor) => Accept<Unit>(visitor);
    }
}
