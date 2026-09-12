using System.Text;

namespace Llyn.Core;

public sealed record LReading(string LReadingVariety, string LReadingPhonetic)
{
    private const string LReadingBoundaries = ".\u00B7\u2027";

    public static string LReadingNormalize(string phonetic)
    {
        string composed = phonetic.Normalize(NormalizationForm.FormC);
        StringBuilder kept = new(composed.Length);
        foreach (char symbol in composed)
        {
            if (!LReadingBoundaries.Contains(symbol) && !char.IsWhiteSpace(symbol))
            {
                kept.Append(symbol);
            }
        }

        return kept.ToString().Trim();
    }
}
