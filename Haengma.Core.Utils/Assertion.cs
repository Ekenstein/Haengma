using System;

namespace Haengma.Core.Utils
{
    public static class Assertion
    {
        /// <summary>
        /// Checks whether the given <paramref name="predicate"/> holds. If it doesn't hold,
        /// an <see cref="ArgumentException"/> will be thrown containing
        /// the message produced from the given <paramref name="errorMessage"/>
        /// function.
        /// </summary>
        /// <exception cref="ArgumentException">If the predicate doesn't hold.</exception>
        public static void Require(bool predicate, string errorMessage) => Assert(
            predicate, 
            () => new ArgumentException(errorMessage)
        );

        /// <summary>
        /// Checks whether the given <paramref name="predicate"/> holds. If it doesn't hold,
        /// an <see cref="InvalidOperationException"/> will be thrown containing
        /// the message produced from the given <paramref name="errorMessage"/> function.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the predicate doesn't hold.</exception>
        public static void Check(bool predicate, string errorMessage) => Assert(
            predicate,
            () => new InvalidOperationException(errorMessage)
        );

        public static void Assert(bool predicate, Func<Exception> ex)
        {
            if (!predicate)
            {
                throw ex();
            }
        }
    }
}
