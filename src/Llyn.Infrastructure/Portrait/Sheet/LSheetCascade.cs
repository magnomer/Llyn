using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetCascade
{
    private static readonly (string, string)[] _lSheetCascadeRules =
    [
        ("*", "box-sizing:border-box;"),
        ("html,body", "margin:0;padding:0;"),
        ("body", "background:var(--surface);color:var(--ink);"
            + "font-family:var(--family);font-size:14px;line-height:1.45;"
            + "-webkit-print-color-adjust:exact;print-color-adjust:exact;"),
        (".entry", "display:block;margin:0;padding:18px 10px 0 30px;text-align:left;"),

        (".header", "display:grid;grid-template-columns:auto auto auto;justify-content:start;"
            + "align-items:center;min-height:56px;line-height:normal;"),
        (".headword", "grid-column:1;grid-row:1;align-self:center;margin:0;font-size:40px;font-weight:700;"),
        (".reading", "grid-column:1;grid-row:2;margin:2px 0 0 2px;"
            + "font-family:var(--family);font-size:16px;color:var(--muted);"),
        (".language", "grid-column:2;grid-row:1;align-self:center;justify-self:start;"
            + "display:inline-flex;align-items:center;margin:0 0 0 14px;padding:4px 9px;"
            + "background:var(--accent-soft);border:1px solid var(--accent-soft);border-radius:12px;"),
        (".language>.flag,.language>.globe", "display:inline-flex;align-items:center;justify-content:center;"
            + "width:19px;height:14px;margin:0 8px 0 0;"),
        (".language>.globe::before", "content:\"\";width:12px;height:12px;"
            + "border:1.2px solid var(--accent);border-radius:50%;"),
        (".language>.name", "font-size:12px;font-weight:600;color:var(--accent);"),
        (".language::after", "content:\"\";width:12px;height:12px;margin:0 0 0 8px;"),
        (".flag", "background-position:center;background-size:contain;background-repeat:no-repeat;"),

        (".regard", "grid-column:3;grid-row:1;align-self:center;display:flex;align-items:center;height:42px;"),
        (".heart", "display:inline-block;width:42px;height:42px;margin:0 0 0 12px;"
            + "background-position:center;background-size:24px 24px;background-repeat:no-repeat;"),
        (".grasp", "display:inline-flex;align-items:center;gap:3px;margin:0 0 0 2px;padding:4px;"),
        (".star", "display:inline-block;width:16px;height:16px;"
            + "background-color:color-mix(in srgb,var(--accent) 55%,transparent);"
            + "mask-position:center;mask-size:16px 16px;mask-repeat:no-repeat;"),
        (".star.full", "background-color:var(--accent);"),
        (".star.half", "background-image:linear-gradient(90deg,var(--accent) 50%,"
            + "color-mix(in srgb,var(--accent) 55%,transparent) 50%);"),
        (".grasp:not(:has(.full,.half))>.star",
            "background-color:color-mix(in srgb,var(--muted) 35%,transparent);"),
        (".rating", "margin:0 0 0 6px;font-size:12px;font-weight:600;color:var(--muted);"),

        (".sound", "width:fit-content;margin:12px 0 0 12px;line-height:normal;"),
        ("summary", "list-style:none;cursor:pointer;"),
        ("summary::-webkit-details-marker", "display:none;"),
        ("table.reflex", "border-collapse:collapse;border-spacing:0;"),
        ("table.reflex tr", "height:31px;"),
        ("table.reflex th,table.reflex td", "padding:0;vertical-align:middle;white-space:nowrap;"),
        ("table.reflex .label,table.reflex .kind", "text-align:right;font-size:11px;font-weight:400;"
            + "color:var(--muted);"),
        ("table.reflex .label", "padding-right:24px;"),
        ("table.reflex .kind", "padding-right:6px;max-width:160px;overflow:hidden;text-overflow:ellipsis;"),
        ("table.reflex .text", "position:relative;top:-3px;padding:0 3px;max-width:426px;"
            + "overflow:hidden;text-overflow:ellipsis;"
            + "font-family:var(--phonetic);font-size:19px;font-weight:500;color:var(--ink);"),
        ("table.reflex .romanization,table.reflex .meaning,table.reflex .note,table.reflex .anchor",
            "min-width:12px;max-width:420px;overflow:hidden;text-overflow:ellipsis;padding:0 3px 2px 6px;"
            + "font-family:var(--phonetic);font-size:13px;font-weight:400;color:var(--muted);"),
        ("table.reflex .romanization,table.reflex .anchor", "padding-left:12px;"),
        ("table.reflex .anchor", "max-width:520px;"),
        ("table.reflex .anchor:empty", "padding:0;min-width:0;"),
        ("details.fold>summary", "display:flex;align-items:center;width:fit-content;margin:2px auto 0 auto;"
            + "padding:2px 4px;border-radius:6px;font-family:var(--family);font-size:11px;color:var(--muted);"),
        ("details.fold>summary::before", "content:\"\";width:12px;height:12px;margin:0 4px 0 0;"
            + "background-position:center;background-size:12px 12px;background-repeat:no-repeat;"),
        ("details.fold[open]>summary::before", "transform:rotate(180deg);"),
        (".pronunciation", "border-collapse:collapse;border-spacing:0;"),
        (".pronunciation tr", "height:37px;"),
        (".pronunciation th,.pronunciation td", "padding:0;vertical-align:middle;white-space:nowrap;"),
        (".pronunciation th", "text-align:right;padding-right:24px;"
            + "font-size:11px;font-weight:400;color:var(--muted);"),
        (".pronunciation th:empty,.pronunciation th:has(.flag)", "padding-right:0;"),
        (".pronunciation .flag", "display:inline-block;vertical-align:middle;"
            + "width:19px;height:14px;margin:0 8px 1px 0;"),
        (".pronunciation .bracket", "display:inline-block;vertical-align:middle;position:relative;top:-3px;"
            + "font-size:19px;color:var(--muted);"),
        (".pronunciation .text", "display:inline-block;vertical-align:middle;position:relative;top:-3px;"
            + "margin:0 3px;max-width:292px;overflow:hidden;text-overflow:ellipsis;"
            + "font-family:var(--phonetic);font-size:19px;font-weight:500;color:var(--ink);"),
        (".pronunciation .cell", "display:inline-block;margin:0 3px;"),

        (".entry>.speech", "margin:14px 0 -4px 12px;line-height:normal;"),
        (".scope", "display:flex;"),
        (".scope>span", "margin:0 6px 7px 0;font-size:12px;font-weight:600;color:var(--muted);"),
        (".chips", "display:flex;flex-wrap:wrap;"),
        (".chip", "display:inline-flex;align-items:center;height:28px;margin:0 7px 7px 0;padding:0 12px;"
            + "border-radius:14px;background:var(--accent-soft);"
            + "font-size:12px;font-weight:600;color:var(--accent);"),

        (".frequency", "margin:14px 0 0 12px;line-height:normal;"),
        (".frequency>.pill", "display:inline-flex;align-items:center;height:28px;margin:0 7px 7px 0;"
            + "padding:0 12px;border-radius:14px;background:var(--surface-raised);"),
        (".frequency .tier", "font-size:12px;font-weight:600;color:var(--muted);"),
        (".frequency .band", "margin:0 0 0 7px;font-size:11px;"),
        (".frequency .pip", "color:color-mix(in srgb,var(--muted) 22%,transparent);"),
        (".frequency-core .tier,.frequency-core .pip.on", "color:var(--frequency-core);"),
        (".frequency-everyday .tier,.frequency-everyday .pip.on", "color:var(--frequency-everyday);"),
        (".frequency-advanced .tier,.frequency-advanced .pip.on", "color:var(--frequency-advanced);"),
        (".frequency-rare .tier,.frequency-rare .pip.on", "color:var(--frequency-rare);"),

        (".paradigm", "display:block;width:fit-content;margin:14px 0 0 0;padding:14px 16px;"
            + "border:1px solid var(--line);border-radius:12px;background:transparent;line-height:normal;"),
        (".paradigm>input", "display:none;"),
        ("#paradigm-short:checked~.inflection-full,#paradigm-full:checked~.inflection-short", "display:none;"),
        (".paradigm table", "border-collapse:collapse;border-spacing:0;"),
        (".paradigm td,.paradigm th", "padding:0;vertical-align:middle;white-space:nowrap;text-align:left;"),
        (".paradigm-rows td", "padding-bottom:4px;font-size:12px;"),
        (".paradigm-rows .part", "padding-right:14px;font-weight:600;color:var(--accent);"),
        (".paradigm-rows .part:empty", "padding-right:0;"),
        (".paradigm-rows .name", "padding-right:14px;color:var(--muted);"),
        (".paradigm-rows .text", "font-size:14px;color:var(--ink);"),
        (".paradigm-rows .text[title]", "color:var(--muted);"),
        (".paradigm .switch", "display:flex;width:fit-content;margin:0 0 10px auto;padding:2px;"
            + "border-radius:9px;background:var(--surface-raised);"),
        (".paradigm .switch>label", "padding:3px 12px;border:1px solid transparent;border-radius:7px;"
            + "font-size:12px;font-weight:600;color:var(--muted);cursor:pointer;"),
        (".inflection th", "padding:0 18px 6px 0;border-bottom:1px solid var(--line);"
            + "font-size:12px;font-weight:400;color:var(--muted);"),
        (".inflection th:empty", "padding:0;"),
        (".inflection td", "padding:5px 18px 5px 0;font-size:15px;color:var(--ink);"),
        (".inflection td.llyn-group", "padding-right:24px;font-size:14px;font-weight:600;color:var(--accent);"),
        (".inflection td.llyn-label", "font-size:12px;color:var(--muted);"),
        (".inflection td.llyn-group:empty,.inflection td.llyn-label:empty", "padding-right:0;"),
        (".inflection tr.llyn-close>td", "border-top:1px solid var(--line);"),
        (".inflection tr.llyn-rule>td:not(:first-child)", "background-image:linear-gradient(90deg,"
            + "var(--line-strong) 1px,transparent 1px);background-size:3px 1px;background-repeat:repeat-x;"),
        (".llyn-marked", "color:var(--warning);"),
        (".llyn-cut,.llyn-muted", "color:var(--muted);"),

        ("details.box", "margin:14px 0 0 0;padding:12px 18px 10px 14px;border:1px solid var(--line);"
            + "border-radius:10px;background:var(--surface);line-height:normal;"),
        ("details.box.script", "margin-top:28px;padding-bottom:6px;"),
        ("details.box>summary", "display:flex;align-items:center;padding:0 0 4px 0;"
            + "font-size:12px;font-weight:600;color:var(--muted);"),
        ("details.box>summary::after", "content:\"\";flex:0 0 23px;height:23px;margin:0 3px -4px auto;"
            + "border-radius:12px;opacity:.72;"
            + "background-position:center;background-size:12px 12px;background-repeat:no-repeat;"),
        ("details.box[open]>summary::after",
            "opacity:1;background-color:var(--accent-soft);transform:rotate(180deg);"),
        ("details.box>section.band", "margin:0;"),

        ("section.band", "margin:39px 0 0 0;"),
        ("section.band+section.band", "margin-top:28px;"),
        ("section.band>h2", "font-size:19px;font-weight:600;margin:0 2px 12px 2px;padding:5px 0 0 0;"
            + "break-after:avoid;page-break-after:avoid;"),

        (".card", "background:var(--surface);border:1px solid var(--line);"
            + "border-radius:12px;margin:0 0 15px 0;break-inside:avoid;page-break-inside:avoid;"),
        (".card>header,.card>summary", "position:relative;display:flex;align-items:center;height:55px;"
            + "padding:0 22px;background:var(--surface);border-bottom:1px solid var(--line);"
            + "border-radius:12px 12px 0 0;line-height:normal;"),
        ("details.card:not([open])>summary", "border-bottom-width:0;border-radius:12px;"),
        (".rank", "display:flex;align-items:center;justify-content:center;flex:0 0 auto;"
            + "width:28px;height:28px;margin:0 12px 0 0;border:1px solid transparent;border-radius:14px;"
            + "background:var(--accent-soft);font-weight:600;color:var(--accent);"),
        (".title,.caption,.peek", "margin:0 62px 0 0;font-size:16px;white-space:nowrap;"),
        (".title,.caption", "font-weight:600;"),
        (".caption,.peek,.title.kind", "color:var(--muted);"),
        (".peek", "flex:1 1 auto;min-width:0;overflow:hidden;text-overflow:ellipsis;"),
        ("details.card[open] .peek,details.card:not([open]) .caption", "display:none;"),
        ("details.card>summary::after", "content:\"\";position:absolute;right:22px;top:50%;"
            + "width:26px;height:26px;margin:-13px 0 0 0;border-radius:6px;transform:rotate(180deg);"
            + "background-position:center;background-size:12px 12px;background-repeat:no-repeat;"),
        ("details.card:not([open])>summary::after", "transform:none;"),
        (".card>section", "margin:21px 30px 24px 30px;padding-right:76px;"),

        (".phrase", "font-size:16px;font-weight:600;margin:0 0 7px 0;"),
        (".sense", "font-size:16px;margin:0;"),

        (".collocations", "margin:20px 0 0 0;border:1px solid var(--line);border-radius:10px;"),
        (".collocations>h3", "font-size:13px;font-weight:600;color:var(--muted);margin:0;"
            + "padding:9px 16px;border-bottom:1px solid var(--line);"),
        (".collocation", "padding:12px 16px;break-inside:avoid;page-break-inside:avoid;"),
        (".collocation+.collocation", "border-top:1px solid var(--line);"),
        ("details.collocation>summary", "display:flex;align-items:baseline;gap:10px;"),
        ("details.collocation>summary::before", "content:\"\";align-self:center;flex:0 0 12px;height:12px;"
            + "transform:rotate(-90deg);background-position:center;background-size:12px 12px;"
            + "background-repeat:no-repeat;"),
        ("details.collocation[open]>summary::before", "transform:none;"),
        ("details.collocation>summary .phrase", "margin:0;"),

        ("section.band .speech", "margin:14px 0 0 12px;display:flex;flex-wrap:wrap;"),
        ("section.band .speech span", "height:28px;line-height:28px;margin:0 7px 7px 0;padding:0 12px;"
            + "border-radius:14px;background:var(--accent-soft);"
            + "font-size:12px;font-weight:600;color:var(--accent);"),

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

        ("div.note", "background:var(--surface);border:1px solid var(--line);border-radius:12px;"
            + "padding:22px 26px;font-size:14px;line-height:21px;"),
        ("div.note>:first-child", "margin-top:0;"),
        ("div.note>:last-child", "margin-bottom:0;"),
        ("div.note p,div.note ul,div.note ol,div.note blockquote,div.note pre", "margin:0 0 10px;"),
        ("div.note h3,div.note h4,div.note h5,div.note h6", "margin:14px 0 6px;font-size:15px;font-weight:600;"),
        ("div.note ul,div.note ol", "padding-left:22px;"),
        ("div.note li", "margin:2px 0;"),
        ("div.note blockquote", "border-left:3px solid var(--line);padding-left:12px;color:var(--muted);"),
        ("div.note code", "font-family:Consolas,monospace;font-size:13px;background:var(--accent-soft);"
            + "padding:1px 4px;border-radius:4px;"),
        ("div.note pre", "white-space:pre-wrap;background:var(--accent-soft);"
            + "padding:10px 12px;border-radius:8px;"),
        ("div.note pre code", "background:none;padding:0;"),
        ("div.note hr", "border:0;border-top:1px solid var(--line);margin:12px 0;"),
        ("div.note a", "color:var(--accent);"),

        (".stamp", "margin:36px 2px 24px 2px;line-height:normal;"),
        (".stamp>hr", "height:1px;margin:0 0 14px 0;border:0;background:var(--line);"),
        (".stamp>.grid", "display:grid;grid-template-columns:auto 1fr;"),
        (".stamp .label,.stamp .time", "margin:0 0 4px 0;font-size:12px;color:var(--muted);"),
        (".stamp .label", "margin-right:18px;"),
    ];

    private const string LSheetCascadePaper =
        "@page{margin:12mm;}@media print{.entry{padding:0;}}";

    public static string LSheetCascadeRead(LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        StringBuilder sheet = new StringBuilder(LSheetStyle.LSheetVariableFormat(theme));
        foreach ((string selector, string body) in _lSheetCascadeRules)
        {
            sheet.Append(selector).Append('{').Append(body).Append('}');
        }

        sheet.Append(".star{mask-image:url(\"")
            .Append(LPortraitIcon.LPortraitIconRead("star").LPortraitAssetAddress)
            .Append("\")}.heart{background-image:url(\"")
            .Append(LPortraitIcon.LPortraitIconRead("favorite").LPortraitAssetAddress)
            .Append("\")}.heart.on{background-image:url(\"")
            .Append(LPortraitIcon.LPortraitIconRead("favorite-marked").LPortraitAssetAddress)
            .Append("\")}details.card>summary::after,details.box>summary::after,details.fold>summary::before")
            .Append(",details.collocation>summary::before")
            .Append("{background-image:url(\"")
            .Append(LPortraitIcon.LPortraitIconRead("expand").LPortraitAssetAddress)
            .Append("\")}");

        return sheet.Append(LSheetCascadePaper).ToString();
    }
}
