namespace Haengma.Core.Sgf
{
    public interface ISgfPropertyVisitor<T>
    {
        T Accept(SgfProperty.B b);
        T Accept(SgfProperty.W w);
        T Accept(SgfProperty.C c);
        T Accept(SgfProperty.PB pB);
        T Accept(SgfProperty.PW pW);
        T Accept(SgfProperty.AB aB);
        T Accept(SgfProperty.AW aW);
        T Accept(SgfProperty.SZ sZ);
        T Accept(SgfProperty.HA hA);
        T Accept(SgfProperty.MN mN);
        T Accept(SgfProperty.KM kM);
        T Accept(SgfProperty.PL pL);
        T Accept(SgfProperty.AP aP);
        T Accept(SgfProperty.Unknown unknown);
        T Accept(SgfProperty.BR bR);
        T Accept(SgfProperty.WR wR);
        T Accept(SgfProperty.OT oT);
        T Accept(SgfProperty.RE rE);
        T Accept(SgfProperty.Emote emote);
    }
}
