using System;

namespace GTP
{
    /// <summary>
    /// A structured description of a parsed GTP command.
    /// </summary>
    public class GtpCommand
    {
        /// <summary>
        /// The optional identity number associated with this command input.
        /// </summary>
        public int? Id { get; }

        /// <summary>
        /// The name of the command.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// The optional arguments associated with this command input.
        /// </summary>
        public string[] Arguments { get; }

        /// <summary>
        /// Constructs a GTP command.
        /// </summary>
        /// <param name="id">The optional ID of the command.</param>
        /// <param name="command">The name of the command to be executed.</param>
        /// <param name="arguments">The optional arguments associated with this command.</param>
        /// <exception cref="ArgumentException">If <paramref name="command"/> is null or white space.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="arguments"/> is null.</exception>
        public GtpCommand(int? id, string command, string[] arguments)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException("Command must not be null or white space.");
            }

            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            Id = id;
            Command = command;
        }

        public override string ToString() => $"{Id} {Command} {string.Join(" ", Arguments)}";
    }
}
