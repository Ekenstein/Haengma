using Haengma.Core.Sgf;
using Haengma.Core.Utils;
using Haengma.GS.Hubs;
using Haengma.GS.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Haengma.Core.Sgf.SgfProperty;
using static Haengma.Tests.RandomExtensions;

namespace Haengma.Tests
{
    public static class Fixture
    {
        public static readonly IEqualityComparer<SgfProperty> PropertyComparer = EqualityComparer<SgfProperty>
            .Default
            .WithEquals(AreEquals);

        public static readonly IEqualityComparer<SgfNode> NodeComparer = EqualityComparer<SgfNode>
            .Default
            .WithEquals(AreEquals);

        public static readonly IEqualityComparer<SgfGameTree> GameTreeComparer = EqualityComparer<SgfGameTree>
            .Default
            .WithEquals(AreEquals);

        public static readonly IEqualityComparer<IReadOnlyList<SgfGameTree>> CollectionComparer = EqualityComparer<IReadOnlyList<SgfGameTree>>
            .Default
            .WithEquals(AreEquals);

        private static bool AreEquals(IReadOnlyList<SgfGameTree> x, IReadOnlyList<SgfGameTree> y)
        {
            return x.SequenceEqual(y, GameTreeComparer);
        }

        private static bool AreEquals(SgfGameTree x, SgfGameTree y)
        {
            return x.Sequence.SequenceEqual(y.Sequence, NodeComparer) && x.Trees.SequenceEqual(y.Trees, GameTreeComparer);
        }

        private static bool AreEquals(SgfNode x, SgfNode y) => x.Properties.SequenceEqual(y.Properties, PropertyComparer);

        private static bool AreEquals(SgfProperty x, SgfProperty y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x.Type != y.Type) return false;
            return x.Accept(new SgfPropertyEqualsVisitor(y));
        }

        private class SgfPropertyEqualsVisitor : ISgfPropertyVisitor<bool>
        {
            private readonly SgfProperty _other;

            public SgfPropertyEqualsVisitor(SgfProperty other)
            {
                _other = other;
            }

            private bool CheckEquality<T>(Func<T, bool> equals) where T : SgfProperty
            {
                var other = _other as T;
                if (other == null) return false;
                var result = equals(other);
                if (!result)
                {
                    var i = 0;
                }

                return result;
            }

            public bool Accept(B b) => CheckEquality<B>(x => x.Move == b.Move);

            public bool Accept(W w) => CheckEquality<W>(x => x.Move == w.Move);

            public bool Accept(C c) => CheckEquality<C>(x => x.Comment == c.Comment);

            public bool Accept(PB pB) => CheckEquality<PB>(x => x.Name == pB.Name);

            public bool Accept(PW pW) => CheckEquality<PW>(x => x.Name == pW.Name);

            public bool Accept(AB aB) => CheckEquality<AB>(x => x.Stones.SequenceEqual(aB.Stones));

            public bool Accept(AW aW) => CheckEquality<AW>(x => x.Stones.SequenceEqual(aW.Stones));

            public bool Accept(SZ sZ) => CheckEquality<SZ>(x => x.Size == sZ.Size);

            public bool Accept(HA hA) => CheckEquality<HA>(x => x.Handicap == hA.Handicap);

            public bool Accept(MN mN) => CheckEquality<MN>(x => x.MoveNumber == mN.MoveNumber);

            public bool Accept(KM kM) => CheckEquality<KM>(x => x.Komi == kM.Komi);

            public bool Accept(PL pL) => CheckEquality<PL>(x => x.Color == pL.Color);
            public bool Accept(AP aP) => CheckEquality<AP>(x =>
            {
                return x.Application.name == aP.Application.name && x.Application.version == aP.Application.version;
            });

            public bool Accept(Unknown unknown) => CheckEquality<Unknown>(x => 
            {
                return x.Identifier == unknown.Identifier && x.Values.SequenceEqual(unknown.Values);
            });

            public bool Accept(BR bR) => CheckEquality<BR>(x => x.Rank == bR.Rank);

            public bool Accept(WR wR) => CheckEquality<WR>(x => x.Rank == wR.Rank);

            public bool Accept(OT oT) => CheckEquality<OT>(x => x.Overtime == oT.Overtime);

            public bool Accept(RE rE) => CheckEquality<RE>(x => x.Result == rE.Result);

            public bool Accept(Emote emote) => CheckEquality<Emote>(x =>
            {
                return x.Color == emote.Color && x.Message == emote.Message;
            });
        }


        public static readonly Random Rng = new(42);

        public static async Task<string> CreateGameAsync(HubConnection owner, 
            Mock<IGameClient> gameClient, 
            JsonGameSettings gameSettings = null)
        {
            await owner.CreateGameAsync(gameSettings ?? GameSettings());
            return await gameClient.VerifyGameCreated(Times.Once());
        }

        public static JsonTimeSettings TimeSettings(
            JsonTimeSettingType type = JsonTimeSettingType.ByoYomi,
            int mainTimeInSeconds = 60 * 10,
            int byoYomiPeriods = 5,
            int byoYomiSeconds = 30) => new(type, mainTimeInSeconds, byoYomiPeriods, byoYomiSeconds);

        public static JsonGameSettings GameSettings(
            JsonTimeSettings timeSettings = null,
            int boardSize = 19,
            double komi = 6.5,
            int handicap = 0,
            JsonPlayerDecision playerDecision = JsonPlayerDecision.Nigiri
        ) => new(
            boardSize,
            komi,
            handicap,
            timeSettings ?? TimeSettings(),
            playerDecision
        );

        public static IReadOnlyList<SgfGameTree> NextSgfCollection(
            int minTrees = 1,
            int maxTrees = 4
        ) => Rng.NextCollection(NextGameTree, minTrees, maxTrees);

        private static SgfGameTree NextGameTree(Random random)
        {
            var sequence = random.NextCollection(NextSgfNode);
            var trees = random.NextCollection(NextGameTree, minSize: 0, maxSize: 1);
            return new SgfGameTree(sequence, trees);
        }

        private static SgfNode NextSgfNode(Random random)
        {
            var properties = random.NextCollection(NextSgfProperty, minSize: 1).ToSet();
            return new SgfNode(properties);
        }

        private static readonly Func<Random, SgfProperty>[] PropertyGenerators = new Func<Random, SgfProperty>[]
        {
            rng => new B(NextMove(rng)),
            rng => new W(NextMove(rng)),
            rng => new C(rng.NextString()),
            rng => new PB(rng.NextString()),
            rng => new PW(rng.NextString()),
            rng => new AB(rng.NextCollection(NextPoint).ToSet()),
            rng => new AW(rng.NextCollection(NextPoint).ToSet()),
            rng => new SZ(rng.Next()),
            rng => new HA(rng.Next()),
            rng => new MN(rng.Next()),
            rng => new KM(rng.NextDouble()),
            rng => new PL(NextColor(rng)),
            rng => new AP((rng.NextString(), rng.NextString())),
            rng => new BR(rng.NextString()),
            rng => new WR(rng.NextString()),
            rng => new OT(rng.NextString()),
            rng => new RE(rng.NextString()), 
            rng => new Emote(NextColor(rng), rng.Next(Enum.GetValues<SgfEmote>())),
            rng => new Unknown(
                rng.NextString(UCLetters, minLength: 3), 
                rng.NextCollection(x => x.NextString(), maxSize: 4)
            )
        };

        private static SgfProperty NextSgfProperty(Random random) => random.Next(PropertyGenerators).Invoke(random);

        private static SgfColor NextColor(Random random) => random.Next(new [] { SgfColor.Black, SgfColor.White });

        private static Point NextPoint(Random random) => new(random.Next(1, 52), random.Next(1, 52));

        private static Move NextMove(Random random)
        {
            var options = new Move[]
            {
                new Move.Pass(),
                NextPoint(random).Let(x => new Move.Point(x.X, x.Y))
            };
            
            return random.Next(options);
        }
    }
}
