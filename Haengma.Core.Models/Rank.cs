using System;

namespace Haengma.Core.Models
{
    public abstract record Rank
    {
        public abstract int Value { get; init; }

        public sealed record Dan : Rank
        {
            private int _value;
            public override int Value
            {
                get => _value;
                init
                {
                    if (value > 9 || value < 1)
                    {
                        throw new ArgumentException("A dan rank must be between 1 and 9.");
                    }

                    _value = value;
                }
            }
        }

        public sealed record Kyu : Rank
        {
            private int _value;

            public override int Value 
            { 
                get => _value; 
                init
                {
                    if (value > 30 || value < 1)
                    {
                        throw new ArgumentException("A kyu rank must be between 1 and 30.");
                    }

                    _value = value;
                }
            }
        }
    }
}
