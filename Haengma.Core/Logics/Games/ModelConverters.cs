﻿using Haengma.Core.Models;
using Haengma.Core.Sgf;
using System;

namespace Haengma.Core.Logics.Games
{
    public static class ModelConverters
    {
        public static Models.Point ToServiceModel(this Sgf.SgfPoint point) => new(point.X, point.Y);

        public static Models.Stone ToServiceModel(this Sgf.Stone stone) => new(stone.Point.ToServiceModel(), stone.Color.ToServiceModel());

        public static Color ToServiceModel(this SgfColor color) => color switch
        {
            SgfColor.Black => Color.Black,
            SgfColor.White => Color.White,
            _ => throw new InvalidOperationException($"Couldn't recognize the SGF color {color}.")
        };

        public static SgfColor ToSgfModel(this Color color) => color switch
        {
            Color.Black => SgfColor.Black,
            Color.White => SgfColor.White,
            _ => throw new InvalidOperationException($"Couldn't recognize the color {color}.")
        };

        public static SgfEmote ToSgfModel(this Emote emote) => emote switch
        {
            Emote.Greetings => SgfEmote.Greetings,
            Emote.Bye => SgfEmote.Bye,
            Emote.Mistake => SgfEmote.Mistake,
            Emote.Impressed => SgfEmote.Impressed,
            Emote.Thanks => SgfEmote.Thanks,
            _ => throw new ArgumentOutOfRangeException(nameof(emote), emote, $"Couldn't recognize the emote {emote}")
        };
    }
}
