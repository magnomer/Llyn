using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Llyn.Core;

public sealed record LStencilToken(
    string? LStencilTokenLiteral,
    LStencilField? LStencilTokenField,
    int? LStencilTokenIndex,
    bool LStencilTokenAll)
{
    public static IReadOnlyList<LStencilToken> LStencilTokenParse(string body)
    {
        ArgumentNullException.ThrowIfNull(body);

        List<LStencilToken> tokens = [];
        StringBuilder literal = new StringBuilder();
        int place = 0;

        while (place < body.Length)
        {
            int open = body.IndexOf('{', place);
            int close = open < 0 ? -1 : body.IndexOfAny(['{', '}'], open + 1);

            if (close < 0)
            {
                literal.Append(body, place, body.Length - place);
                break;
            }

            if (body[close] == '{')
            {
                literal.Append(body, place, close - place);
                place = close;
                continue;
            }

            literal.Append(body, place, open - place);
            LStencilToken? field = LStencilTokenRead(body.Substring(open + 1, close - open - 1));

            if (field is null)
            {
                literal.Append(body, open, close - open + 1);
            }
            else
            {
                if (literal.Length > 0)
                {
                    tokens.Add(new LStencilToken(literal.ToString(), null, null, false));
                    literal.Clear();
                }

                tokens.Add(field);
            }

            place = close + 1;
        }

        if (literal.Length > 0)
        {
            tokens.Add(new LStencilToken(literal.ToString(), null, null, false));
        }

        return tokens;
    }

    private static LStencilToken? LStencilTokenRead(string inner)
    {
        string[] parts = inner.Split(':');

        if (parts.Length > 2)
        {
            return null;
        }

        LStencilField? field = parts[0].Trim().ToLowerInvariant() switch
        {
            "headword" => LStencilField.LStencilFieldHeadword,
            "forms" => LStencilField.LStencilFieldForms,
            "pronunciation" => LStencilField.LStencilFieldPronunciation,
            "respelling" => LStencilField.LStencilFieldRespelling,
            "recording" => LStencilField.LStencilFieldRecording,
            "speech" => LStencilField.LStencilFieldSpeech,
            "unit" => LStencilField.LStencilFieldUnit,
            "freedom" => LStencilField.LStencilFieldFreedom,
            "definition" => LStencilField.LStencilFieldDefinition,
            "expression" => LStencilField.LStencilFieldExpression,
            "paraphrase" => LStencilField.LStencilFieldParaphrase,
            "sentence" => LStencilField.LStencilFieldSentence,
            "gloss" => LStencilField.LStencilFieldGloss,
            "target" => LStencilField.LStencilFieldTarget,
            "image" => LStencilField.LStencilFieldImage,
            "tag" => LStencilField.LStencilFieldTag,
            "register" => LStencilField.LStencilFieldRegister,
            "situation" => LStencilField.LStencilFieldSituation,
            "source" => LStencilField.LStencilFieldSource,
            _ => null,
        };

        if (field is null)
        {
            return null;
        }

        if (parts.Length == 1)
        {
            return new LStencilToken(null, field, null, false);
        }

        string suffix = parts[1].Trim();

        if (suffix.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return new LStencilToken(null, field, null, true);
        }

        return int.TryParse(suffix, NumberStyles.None, CultureInfo.InvariantCulture, out int index) && index > 0
            ? new LStencilToken(null, field, index, false)
            : null;
    }
}
