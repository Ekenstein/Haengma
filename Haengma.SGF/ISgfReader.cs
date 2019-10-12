using Monadicsh;
using System.IO;
using System.Threading.Tasks;

namespace Haengma.SGF
{
    public interface ISgfReader
    {
        /// <summary>
        /// Tries to parse the given <paramref name="textReader"/> as an <see cref="SgfCollection"/>.
        /// If the parse fails, a <see cref="Result{T}"/> indicating a failure will be returned containing
        /// information of why the given <paramref name="textReader"/> couldn't be parsed.
        /// </summary>
        /// <param name="textReader">The text reader to parse as a <see cref="SgfCollection"/>.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> containing either the parse result where the parse result either can be the parsed
        /// <see cref="SgfCollection"/>, or a collection of errors describing why the <paramref name="textReader"/> couldn't
        /// be parsed to a <see cref="SgfCollection"/>.
        /// </returns>
        Task<Result<SgfCollection>> ReadAsync(TextReader textReader);
    }
}
