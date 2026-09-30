using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LArticulation(
    IReadOnlyList<string> LArticulationHeaders,
    IReadOnlyList<string> LArticulationSides,
    IReadOnlyList<IReadOnlyList<IReadOnlyList<string>>> LArticulationCells)
{
    private static readonly string[] LArticulationLocation =
    [
        "Bilabial",
        "Labiodental",
        "Dental",
        "Alveolar",
        "Postalveolar",
        "Retroflex",
        "Palatal",
        "Velar",
        "Uvular",
        "Pharyngeal",
        "Glottal"
    ];

    private static readonly string[] LArticulationManner =
    [
        "Plosive",
        "Nasal",
        "Trill",
        "Tap",
        "Fricative",
        "LateralFricative",
        "Approximant",
        "LateralApproximant"
    ];

    private static readonly string[,] LArticulationConsonant =
    {
        { "p b", "", "", "t d", "", "ʈ ɖ", "c ɟ", "k ɡ", "q ɢ", "", "ʔ" },
        { "m", "ɱ", "", "n", "", "ɳ", "ɲ", "ŋ", "ɴ", "", "" },
        { "ʙ", "", "", "r", "", "", "", "", "ʀ", "", "" },
        { "", "ⱱ", "", "ɾ", "", "ɽ", "", "", "", "", "" },
        { "ɸ β", "f v", "θ ð", "s z", "ʃ ʒ", "ʂ ʐ", "ç ʝ", "x ɣ", "χ ʁ", "ħ ʕ", "h ɦ" },
        { "", "", "", "ɬ ɮ", "", "", "", "", "", "", "" },
        { "", "ʋ", "", "ɹ", "", "ɻ", "j", "ɰ", "", "", "" },
        { "", "", "", "l", "", "ɭ", "ʎ", "ʟ", "", "", "" }
    };

    private static readonly string[] LArticulationBackness =
    [
        "Front", "Central", "Back"
    ];

    private static readonly string[] LArticulationHeight =
    [
        "Close",
        "NearClose",
        "CloseMid",
        "Mid",
        "OpenMid",
        "NearOpen",
        "Open"
    ];

    private static readonly string[,] LArticulationVowel =
    {
        { "i y", "ɨ ʉ", "ɯ u" },
        { "ɪ ʏ", "", "ʊ" },
        { "e ø", "ɘ ɵ", "ɤ o" },
        { "", "ə", "" },
        { "ɛ œ", "ɜ ɞ", "ʌ ɔ" },
        { "æ", "ɐ", "" },
        { "a ɶ", "ä", "ɑ ɒ" }
    };

    public static LArticulation LArticulationConsonantRead()
    {
        return LArticulationRead(LArticulationLocation, LArticulationManner, LArticulationConsonant);
    }

    public static LArticulation LArticulationVowelRead()
    {
        return LArticulationRead(LArticulationBackness, LArticulationHeight, LArticulationVowel);
    }

    private static LArticulation LArticulationRead(string[] headers, string[] sides, string[,] cells)
    {
        List<IReadOnlyList<IReadOnlyList<string>>> rows = new(sides.Length);
        for (int row = 0; row < sides.Length; row++)
        {
            List<IReadOnlyList<string>> line = new(headers.Length);
            for (int column = 0; column < headers.Length; column++)
            {
                line.Add(cells[row, column].Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }

            rows.Add(line);
        }

        return new LArticulation(headers, sides, rows);
    }
}
