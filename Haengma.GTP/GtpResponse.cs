using Monadicsh;
using Monadicsh.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace GTP
{
    public class GtpResponse
    {
        /// <summary>
        /// Constructs a successful response for the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command that was executed successfully.</param>
        /// <param name="result">The result of executing the command.</param>
        /// <returns>A <see cref="GtpResponse"/> indicating a successful execution of the <paramref name="command"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="result"/> is null or white space.</exception>
        public static GtpResponse Success(int? id, string result) => new GtpResponse(id, true, result);

        /// <summary>
        /// Constructs an error response for the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command that couldn't be executed.</param>
        /// <param name="errorMessage">The error message giving the explanation to why the command couldn't be executed.</param>
        /// <returns>A <see cref="GtpResponse"/> indicating that there were an error executing the <paramref name="command"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="errorMessage"/> is null or white space.</exception>
        public static GtpResponse Error(int? id, string errorMessage) => new GtpResponse(id, false, errorMessage);

        /// <summary>
        /// The command to respond to.
        /// </summary>
        [MaybeNull]public int? Id { get; }

        /// <summary>
        /// Whether the response indicates a successful response or not.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// The message that either is the result of successfully executing the command, or
        /// the error message giving an explanation why the command couldn't be executed.
        /// </summary>
        public string Message { get; }

        private GtpResponse(int? id, bool isSuccess, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("The message must not be null or white space.");
            }

            Id = id;
            IsSuccess = isSuccess;
            Message = message;
        }

        public override string ToString() => $"{(IsSuccess ? "Success" : "Error")}: {Message}";

        /// <summary>
        /// Serializes the response to a GTP response string.
        /// </summary>
        public string Serialize()
        {
            var indicator = IsSuccess ? "=" : "?";
            var id = Maybe.Create(Id).Coalesce(v => v.ToString()).GetValueOrDefault(string.Empty);
            return $"{indicator}{id} {Message}";
        }

        /// <summary>
        /// Throws <see cref="GtpException"/> if the response doesn't indicate a successful
        /// response.
        /// </summary>
        /// <exception cref="GtpException">If the response was unsucessful.</exception>
        public void EnsureSuccessResponse()
        {
            if (!IsSuccess)
            {
                throw new GtpException(Message);
            }
        }
    }
}
