namespace Llyn.UIDeportment;

public partial class PArticulation
{
    private static readonly string[] PConsonantLocation =
    [
        "Articulation.Bilabial",
        "Articulation.Labiodental",
        "Articulation.Dental",
        "Articulation.Alveolar",
        "Articulation.Postalveolar",
        "Articulation.Retroflex",
        "Articulation.Palatal",
        "Articulation.Velar",
        "Articulation.Uvular",
        "Articulation.Pharyngeal",
        "Articulation.Glottal"
    ];

    private static readonly string[] PConsonantManner =
    [
        "Articulation.Plosive",
        "Articulation.Nasal",
        "Articulation.Trill",
        "Articulation.Tap",
        "Articulation.Fricative",
        "Articulation.LateralFricative",
        "Articulation.Approximant",
        "Articulation.LateralApproximant"
    ];

    private static readonly string[,] PConsonantCharacter =
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

    private void PConsonantBuild()
    {
        PArticulationTableBuild(PConsonant, PConsonantLocation.Length, PConsonantManner.Length);

        for (int column = 0; column < PConsonantLocation.Length; column++)
        {
            PArticulationHeaderPlace(PConsonant, PConsonantLocation[column], column);
        }

        for (int row = 0; row < PConsonantManner.Length; row++)
        {
            PArticulationSidePlace(PConsonant, PConsonantManner[row], row);

            for (int column = 0; column < PConsonantLocation.Length; column++)
            {
                PArticulationCellPlace(PConsonant, PConsonantCharacter[row, column], column, row);
            }
        }
    }
}
