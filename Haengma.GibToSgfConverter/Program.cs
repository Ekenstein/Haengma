using CommandLine;
using Haengma.GIB;
using Haengma.GIB.Parser;
using Haengma.SGF;
using Haengma.SGF.SgfProperties;
using Haengma.SGF.ValueTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Haengma.GibToSgfConverter
{
    class Program
    {
        public class Options
        {
            [Option('i', "input", Required = true, HelpText = "The GIB-file to convert to a SGF-file.")]
            public string? InputFile { get; set; }

            [Option('o', "output", Required = true, HelpText = "The destination file where the resulting SGF-file should be saved.")]
            public string? OutputFile { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(OnError);
        }

        static void RunOptions(Options options)
        {
            var input = OpenFile(options.InputFile);
            if (input == null)
            {
                Console.Error.WriteLine("Could not find the GIB-file.");
            }
            else
            {
                using var reader = new StreamReader(input);
                var result = GibParser.Parse(reader);
                if (!result.Success)
                {
                    Console.Error.WriteLine(result.Error?.RenderErrorMessage() ?? "Failed to parse the GIB-file");
                }

                var sgf = GibToSgf(result.Value);
                SaveSgf(options.OutputFile, sgf);
                Console.WriteLine("Converted the GIB-file to SGF successfully.");
            }
        }

        private static void SaveSgf(string? output, SgfGameTree sgf)
        {
            if (string.IsNullOrWhiteSpace(output))
            {
                throw new ArgumentException("Output path must not be null or white space.");
            }

            using var fs = new FileStream(output, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(fs);
            var sgfWriter = new SgfWriter();
            sgfWriter.Write(writer, new SgfCollection(new [] { sgf }));
        }

        private static SgfNode GetRootNode(GibFile gib)
        {
            var properties = new SgfProperty[] { GameType.Go, new Size(19) }
                .Concat(gib.GetBlackPlayerName())
                .Concat(gib.GetWhitePlayerName())
                .Concat(gib.GetHandicap())
                .Concat(gib.GetKomi())
                .Concat(gib.GetResult())
                .Concat(gib.GetGamePlace())
                .Concat(gib.GetGameDate())
                .Concat(gib.GetTimeSettings())
                .Concat(gib.GetBlackRank())
                .Concat(gib.GetWhiteRank());

            return new SgfNode(properties);
        }

        private static IEnumerable<SgfNode> GetMoveNodes(GibFile gib)
        {
            static SgfColor ToColor(GibColor color) => color switch
            {
                GibColor.Black => SgfColor.Black,
                _ => SgfColor.White
            };

            static SgfNode ToNode(GibStone stone) => new SgfNode
            {
                new ExecuteMove(ToColor(stone.Color), new SgfPoint(stone.Coordinates.x, stone.Coordinates.y)),
                new MoveNumber(stone.MoveNumber)
            };

            return gib.Stones.Select(ToNode);
        }

        private static SgfGameTree GibToSgf(GibFile gib)
        {
            var rootNode = GetRootNode(gib);
            var moveNodes = GetMoveNodes(gib);

            return new SgfGameTree(new[] { rootNode }.Concat(moveNodes));
        }

        private static FileStream? OpenFile(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path must not be null or white space.");
            }

            if (!File.Exists(path))
            {
                return null;
            }
            else
            {
                return File.OpenRead(path);
            }
        }

        static void OnError(IEnumerable<Error> errors)
        {

        }
    }
}
