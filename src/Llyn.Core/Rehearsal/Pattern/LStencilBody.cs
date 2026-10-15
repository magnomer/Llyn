using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public static class LStencilBody
{
    public static string LStencilBodyAppend(string body, LStencilField field)
    {
        ArgumentNullException.ThrowIfNull(body);

        string token = new LStencilToken(null, field, null, false).LStencilTokenFormat();
        return body.Length == 0 || body.EndsWith('\n') ? body + token : body + "\n" + token;
    }

    public static string LStencilBodyRemove(string body, int place)
    {
        ArgumentNullException.ThrowIfNull(body);

        List<LStencilToken> tokens = [.. LStencilToken.LStencilTokenParse(body)];
        int at = LStencilBodyFind(tokens, place);
        Func<int, bool> broken = near =>
            near >= 0 && near < tokens.Count
            && tokens[near].LStencilTokenLiteral is string text && text.Trim('\r', '\n').Length == 0;

        if (broken(at - 1))
        {
            tokens.RemoveRange(at - 1, 2);
        }
        else if (broken(at + 1))
        {
            tokens.RemoveRange(at, 2);
        }
        else
        {
            tokens.RemoveAt(at);
        }

        return string.Concat(tokens.Select(token => token.LStencilTokenFormat()));
    }

    public static string LStencilBodySet(string body, int place, bool all)
    {
        ArgumentNullException.ThrowIfNull(body);

        List<LStencilToken> tokens = [.. LStencilToken.LStencilTokenParse(body)];
        int at = LStencilBodyFind(tokens, place);
        tokens[at] = tokens[at] with { LStencilTokenIndex = null, LStencilTokenAll = all };
        return string.Concat(tokens.Select(token => token.LStencilTokenFormat()));
    }

    public static bool LStencilBodyCheck(string body)
    {
        ArgumentNullException.ThrowIfNull(body);

        return LStencilToken.LStencilTokenParse(body).All(token => token.LStencilTokenField is null
            ? token.LStencilTokenLiteral is string text && text.Trim('\r', '\n').Length == 0
            : token.LStencilTokenIndex is null);
    }

    private static int LStencilBodyFind(IReadOnlyList<LStencilToken> tokens, int place)
    {
        int seen = -1;
        for (int at = 0; at < tokens.Count; at++)
        {
            if (tokens[at].LStencilTokenField is not null && ++seen == place)
            {
                return at;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(place), place, "No field token stands at this place.");
    }
}
