namespace Llyn.UIVeneer;

public partial class PArticulation
{
    private static readonly string[] PVowelBackness =
    [
        "Articulation.Front", "Articulation.Central", "Articulation.Back"
    ];

    private static readonly string[] PVowelHeight =
    [
        "Articulation.Close",
        "Articulation.NearClose",
        "Articulation.CloseMid",
        "Articulation.Mid",
        "Articulation.OpenMid",
        "Articulation.NearOpen",
        "Articulation.Open"
    ];

    private static readonly string[,] PVowelCharacter =
    {
        { "i y", "ɨ ʉ", "ɯ u" },
        { "ɪ ʏ", "", "ʊ" },
        { "e ø", "ɘ ɵ", "ɤ o" },
        { "", "ə", "" },
        { "ɛ œ", "ɜ ɞ", "ʌ ɔ" },
        { "æ", "ɐ", "" },
        { "a ɶ", "ä", "ɑ ɒ" }
    };

    private void PVowelBuild()
    {
        PArticulationTableBuild(PVowel, PVowelBackness.Length, PVowelHeight.Length);

        for (int column = 0; column < PVowelBackness.Length; column++)
        {
            PArticulationHeaderPlace(PVowel, PVowelBackness[column], column);
        }

        for (int row = 0; row < PVowelHeight.Length; row++)
        {
            PArticulationSidePlace(PVowel, PVowelHeight[row], row);

            for (int column = 0; column < PVowelBackness.Length; column++)
            {
                PArticulationCellPlace(PVowel, PVowelCharacter[row, column], column, row);
            }
        }
    }
}
