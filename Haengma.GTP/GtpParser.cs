using System;
using System.Linq;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace GTP
{
    public static class GtpParser
    {
        private static readonly char[] Whitespaces = new[] { (char)32, (char)9 };

        /// <summary>
        /// If a char is either SPACE (dec 32) or HT (dec 9) it is considered a white space.
        /// </summary>
        public static Parser<char, char> GtpWhitespace { get; } = Token(Whitespaces.Contains).Labelled("whitespace");

        /// <summary>
        /// Skips chars that are either SPACE (dec 32) or HT (dec 9).
        /// </summary>
        public static Parser<char, Unit> SkipGtpWhitespace { get; } = GtpWhitespace.SkipMany();

        private static Parser<char, int?> Id { get; } = Token(char.IsDigit)
            .ManyString()
            .Assert(v => int.TryParse(v, out var _))
            .Optional()
            .Map(v => v.Match(v => new int?(int.Parse(v)), () => default))
            .Labelled("id");

        private static Parser<char, string> Argument { get; } = Token(_ => true)
            .ManyString()
            .Labelled("argument");

        private static Parser<char, string> ResponseMessage { get; } = Any
            .ManyString()
            .Assert(v => !string.IsNullOrWhiteSpace(v))
            .Labelled("message");

        private static string[] ToArguments(string s) => string.IsNullOrWhiteSpace(s)
            ? new string[0]
            : s.Split(' ').ToArray();

        private static Parser<char, string[]> Arguments { get; } = Argument
            .Map(ToArguments);

        private static Parser<char, string> Command { get; } = Token(char.IsLetterOrDigit)
            .ManyString()
            .Assert(v => !string.IsNullOrWhiteSpace(v))
            .Labelled("command");

        private static Parser<char, GtpResponse> SuccessResponse { get; } =
            from indicator in Token('=').Between(SkipGtpWhitespace)
            from id in Id.Between(SkipGtpWhitespace)
            from message in ResponseMessage.Between(SkipGtpWhitespace)
            select GtpResponse.Success(id, message);

        private static Parser<char, GtpResponse> ErrorResponse { get; } =
            from indicator in Token('?').Between(SkipGtpWhitespace)
            from id in Id.Between(SkipGtpWhitespace)
            from message in ResponseMessage.Between(SkipGtpWhitespace)
            select GtpResponse.Error(id, message);

        private static Parser<char, GtpCommand> GtpCommand { get; } =
            from id in Try(Id.Between(SkipGtpWhitespace))
            from command in Command.Between(SkipGtpWhitespace)
            from argument in Arguments.Between(SkipGtpWhitespace)
            select new GtpCommand(id, command, argument.ToArray());

        public static Result<char, GtpCommand> ParseCommand(string input) => GtpCommand
            .Between(SkipGtpWhitespace)
            .Parse(input);

        public static Result<char, GtpResponse> ParseResponse(string input) => SuccessResponse
            .Or(ErrorResponse)
            .Between(SkipGtpWhitespace)
            .Parse(input);
    }
}
