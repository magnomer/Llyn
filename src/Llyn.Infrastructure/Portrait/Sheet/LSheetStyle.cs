using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetStyle
{
    private static readonly (string, string)[] _lSheetStyleRules =
    [
        ("*", "box-sizing:border-box;"),
        ("html,body", "margin:0;padding:0;"),
        ("body", "background:var(--canvas);color:var(--ink);"
            + "font-family:var(--family);font-size:14px;line-height:1.45;"
            + "-webkit-print-color-adjust:exact;print-color-adjust:exact;"),
        (".portrait", "max-width:960px;margin:0 auto;padding:34px 30px 40px 30px;"),

        (".crest", "display:flex;align-items:center;flex-wrap:wrap;gap:14px;"),
        (".headword", "font-size:40px;font-weight:700;margin:0;line-height:1.15;"),
        (".tongue", "display:inline-flex;align-items:center;gap:8px;"
            + "background:var(--accent-soft);border-radius:12px;padding:5px 16px 5px 10px;"),
        (".tongue span", "font-size:12px;font-weight:600;color:var(--accent);"),
        (".tongue i", "width:12px;height:12px;border-radius:50%;"
            + "border:1.2px solid var(--accent);display:inline-block;"),
        (".star", "font-size:17px;color:var(--accent);line-height:1;"),

        (".sound", "margin:12px 0 0 12px;font-size:19px;"),
        (".sound b", "font-weight:500;color:var(--ink);margin:0 3px;"),
        (".sound em", "font-style:normal;color:var(--muted);"),
        (".sound span", "font-size:14px;color:var(--muted);"),

        (".speech", "margin:14px 0 0 12px;display:flex;flex-wrap:wrap;"),
        (".speech span", "height:28px;line-height:28px;margin:0 7px 7px 0;padding:0 12px;"
            + "border-radius:14px;background:var(--accent-soft);"
            + "font-size:12px;font-weight:600;color:var(--accent);"),

        (".band", "margin:39px 0 0 0;"),
        (".band+.band", "margin-top:28px;"),
        (".band>h2", "font-size:19px;font-weight:600;margin:0 2px 12px 2px;padding:5px 0 0 0;"
            + "break-after:avoid;page-break-after:avoid;"),

        (".card", "background:var(--surface);border:1px solid var(--line);"
            + "border-radius:12px;margin:0 0 15px 0;break-inside:avoid;page-break-inside:avoid;"),
        (".card>header", "display:flex;align-items:center;height:55px;padding:0 22px;"
            + "border-bottom:1px solid var(--line);border-radius:12px 12px 0 0;"),
        (".rank", "width:28px;height:28px;border-radius:14px;background:var(--accent-soft);"
            + "color:var(--accent);font-weight:600;margin-right:12px;flex:0 0 auto;"
            + "display:flex;align-items:center;justify-content:center;"),
        (".title", "font-size:16px;font-weight:600;padding-right:34px;"),
        (".title.kind", "color:var(--muted);"),
        (".card>section", "margin:21px 30px 24px 30px;padding-right:76px;"),

        (".phrase", "font-size:16px;font-weight:600;margin:0 0 7px 0;"),
        (".sense", "font-size:16px;margin:0;"),

        (".scene", "margin:18px 0 0 0;display:flex;flex-wrap:wrap;"),
        (".scene span", "margin:0 9px 7px 0;padding:4px 10px;border-radius:11px;"
            + "background:var(--situation-soft);border:1px solid var(--situation-edge);"
            + "font-size:13px;color:var(--situation);"),

        (".tone", "margin:10px 0 0 0;display:flex;flex-wrap:wrap;"),
        (".scene+.tone", "margin-top:4px;"),
        (".tone span", "margin:0 9px 7px 0;padding:4px 10px;border-radius:11px;"
            + "background:transparent;border:1px solid var(--line);"
            + "font-size:13px;color:var(--muted);"),

        (".bridge", "margin:15px 0 0 0;display:flex;flex-wrap:wrap;align-items:center;"),
        (".scene+.bridge", "margin-top:6px;"),
        (".bridge span", "margin:0 9px 7px 0;padding:5px 10px;border-radius:13px;"
            + "background:var(--accent-soft);display:inline-flex;align-items:center;gap:7px;"),
        (".bridge b", "font-size:13px;font-weight:600;color:var(--accent);"),
        (".bridge i", "font-style:normal;font-size:11px;color:var(--muted);"),

        (".quotes", "margin:16px 0 0 0;padding:1px 0 0 14px;"),
        (".quote", "display:flex;align-items:flex-start;margin:0 0 7px 0;"),
        (".quote>.dot", "color:var(--muted);font-size:15px;margin-right:9px;"),
        (".quote>.frame", "color:var(--accent);font-size:15px;font-weight:700;"
            + "margin-right:16px;white-space:nowrap;"),
        (".quote>.said", "font-family:var(--serif);font-size:15px;"),

        (".labels", "margin:24px 0 0 0;display:flex;flex-wrap:wrap;"),
        (".labels span", "margin:0 7px 7px 0;padding:4px 10px;border-radius:11px;"
            + "border:1px solid var(--line);font-size:12px;color:var(--muted);"),

        (".plates", "margin:16px 0 0 14px;"),
        (".plate", "display:inline-block;background:var(--surface);border:1px solid var(--line);"
            + "border-radius:9px;padding:7px;margin:0 0 7px 0;max-width:100%;"
            + "break-inside:avoid;page-break-inside:avoid;"),
        (".plate img", "display:block;max-width:100%;max-height:460px;height:auto;border-radius:3px;"),
        (".reel", "position:relative;display:block;text-decoration:none;color:inherit;"),
        (".reel .glyph", "position:absolute;left:50%;top:50%;transform:translate(-50%,-50%);"
            + "width:52px;height:52px;border-radius:26px;background:rgba(23,32,51,.66);"
            + "color:#fff;font-size:19px;line-height:52px;text-align:center;"),
        (".reel .said", "display:block;font-size:12px;color:var(--muted);"
            + "margin-top:6px;word-break:break-all;"),
        (".blank", "display:block;width:320px;height:180px;border-radius:3px;"
            + "background:var(--accent-soft);overflow:hidden;"),
        (".blank img", "display:block;width:100%;height:100%;max-height:none;object-fit:cover;"),

        (".lines", "margin:0 2px;"),
        (".line", "display:flex;align-items:flex-start;gap:8px;margin:0 0 5px 0;font-size:16px;"),
        (".line>.tag", "flex:0 0 auto;padding:1px 8px;border-radius:9px;"
            + "background:var(--accent-soft);font-size:12px;font-weight:600;color:var(--accent);"),

        (".rows", "margin:0;"),
        (".row", "display:flex;align-items:center;gap:12px;background:var(--surface);"
            + "border:1px solid var(--line);border-radius:12px;padding:14px 16px;margin:0 0 8px 0;"
            + "break-inside:avoid;page-break-inside:avoid;"),
        (".row>.mark", "width:32px;height:32px;border-radius:10px;background:var(--accent-soft);"
            + "color:var(--accent);text-align:center;line-height:32px;flex:0 0 auto;"),
        (".row>.body", "flex:1 1 auto;min-width:0;"),
        (".row>.body b", "display:block;font-size:15px;font-weight:600;"),
        (".row>.body span", "display:block;font-size:12px;color:var(--muted);margin-top:3px;"),
        (".row>.pill", "padding:3px 9px;border-radius:9px;background:var(--accent-soft);"
            + "font-size:11px;color:var(--accent);flex:0 0 auto;"),
        (".row>.speak", "font-size:12px;color:var(--muted);flex:0 0 auto;"),

        (".note", "background:var(--surface);border:1px solid var(--line);border-radius:12px;"
            + "padding:22px 26px;font-size:14px;line-height:21px;"),
        (".note>:first-child", "margin-top:0;"),
        (".note>:last-child", "margin-bottom:0;"),
        (".note p,.note ul,.note ol,.note blockquote,.note pre", "margin:0 0 10px;"),
        (".note h3,.note h4,.note h5,.note h6", "margin:14px 0 6px;font-size:15px;font-weight:600;"),
        (".note ul,.note ol", "padding-left:22px;"),
        (".note li", "margin:2px 0;"),
        (".note blockquote", "border-left:3px solid var(--line);padding-left:12px;color:var(--muted);"),
        (".note code", "font-family:Consolas,monospace;font-size:13px;background:var(--accent-soft);"
            + "padding:1px 4px;border-radius:4px;"),
        (".note pre", "white-space:pre-wrap;background:var(--accent-soft);"
            + "padding:10px 12px;border-radius:8px;"),
        (".note pre code", "background:none;padding:0;"),
        (".note hr", "border:0;border-top:1px solid var(--line);margin:12px 0;"),
        (".note a", "color:var(--accent);"),
    ];

    private static readonly (string, string)[] _lSheetScopeRules =
    [
        ("h1,h2", "border-bottom:none;padding-bottom:0;"),
        ("ul,ol", "margin-left:0;"),
    ];

    private const string LSheetStylePaper =
        "@page{margin:12mm;}@media print{body{background:#fff;}.portrait{padding:0;max-width:none;}}";

    public static string LSheetStyleRead(LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        return LSheetStyleBuild(theme, null);
    }

    public static string LSheetStyleFormat(LTheme theme, string scope)
    {
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentNullException.ThrowIfNull(scope);
        if (!LSheetScopeCheck(scope))
        {
            throw new ArgumentException("The scope must be a single class selector.", nameof(scope));
        }

        return LSheetStyleBuild(theme, scope);
    }

    private static string LSheetStyleBuild(LTheme theme, string? scope)
    {
        StringBuilder sheet = new StringBuilder();

        sheet.Append(scope ?? ":root").Append('{');
        LSheetVariableAppend(sheet, "ink", theme.LThemeRead("ink"), scope);
        LSheetVariableAppend(sheet, "muted", theme.LThemeRead("muted"), scope);
        LSheetVariableAppend(sheet, "canvas", theme.LThemeRead("canvas"), scope);
        LSheetVariableAppend(sheet, "surface", theme.LThemeRead("surface"), scope);
        LSheetVariableAppend(sheet, "line", theme.LThemeRead("line"), scope);
        LSheetVariableAppend(sheet, "accent", theme.LThemeRead("accent"), scope);
        LSheetVariableAppend(sheet, "accent-soft", theme.LThemeRead("accentSoft"), scope);
        LSheetVariableAppend(sheet, "situation", theme.LThemeRead("situation"), scope);
        LSheetVariableAppend(sheet, "situation-soft", theme.LThemeRead("situationSoft"), scope);
        LSheetVariableAppend(sheet, "situation-edge", theme.LThemeRead("situationEdge"), scope);
        LSheetVariableAppend(sheet, "family", theme.LThemeFamily, scope);
        LSheetVariableAppend(sheet, "serif", theme.LThemeSerif, scope);
        sheet.Append('}');

        foreach ((string selector, string body) in _lSheetStyleRules)
        {
            sheet.Append(scope is null ? selector : LSheetScopeFormat(selector, scope))
                .Append('{').Append(body).Append('}');
        }

        if (scope is null)
        {
            sheet.Append(LSheetStylePaper);
        }
        else
        {
            foreach ((string selector, string body) in _lSheetScopeRules)
            {
                sheet.Append(LSheetScopeFormat(selector, scope)).Append('{').Append(body).Append('}');
            }
        }

        return sheet.ToString();
    }

    private static void LSheetVariableAppend(StringBuilder sheet, string name, string value, string? scope)
    {
        sheet.Append("--").Append(name).Append(':')
            .Append(scope is null ? value : LSheetValueNormalize(value))
            .Append(';');
    }

    private static string LSheetValueNormalize(string value)
    {
        StringBuilder clean = new StringBuilder(value.Length);
        foreach (char letter in value)
        {
            if (letter is not ('{' or '}' or ';' or '<' or '`'))
            {
                clean.Append(letter);
            }
        }

        string text = clean.ToString();
        while (text.Contains("/*", StringComparison.Ordinal))
        {
            text = text.Replace("/*", string.Empty, StringComparison.Ordinal);
        }

        return text;
    }

    private static bool LSheetScopeCheck(string scope)
    {
        if (scope.Length < 2 || scope[0] != '.' || !char.IsAsciiLetter(scope[1]))
        {
            return false;
        }

        for (int index = 2; index < scope.Length; index++)
        {
            char letter = scope[index];
            if (!char.IsAsciiLetterOrDigit(letter) && letter != '-' && letter != '_')
            {
                return false;
            }
        }

        return true;
    }

    private static string LSheetScopeFormat(string selector, string scope)
    {
        switch (selector)
        {
            case "*":
                return scope + "," + scope + " *";
            case "html,body":
            case "body":
                return scope;
            default:
                return string.Join(',', Array.ConvertAll(selector.Split(','), part => scope + " " + part));
        }
    }
}
