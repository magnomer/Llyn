using System;
using System.Text;

namespace Llyn.Infrastructure;

public static class LSheetStyle
{
    public static string LSheetStyleRead(LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        StringBuilder sheet = new StringBuilder();

        sheet.Append(":root{")
            .Append("--ink:").Append(theme.LThemeRead("ink")).Append(';')
            .Append("--muted:").Append(theme.LThemeRead("muted")).Append(';')
            .Append("--canvas:").Append(theme.LThemeRead("canvas")).Append(';')
            .Append("--surface:").Append(theme.LThemeRead("surface")).Append(';')
            .Append("--line:").Append(theme.LThemeRead("line")).Append(';')
            .Append("--accent:").Append(theme.LThemeRead("accent")).Append(';')
            .Append("--accent-soft:").Append(theme.LThemeRead("accentSoft")).Append(';')
            .Append("--situation:").Append(theme.LThemeRead("situation")).Append(';')
            .Append("--situation-soft:").Append(theme.LThemeRead("situationSoft")).Append(';')
            .Append("--situation-edge:").Append(theme.LThemeRead("situationEdge")).Append(';')
            .Append("--family:").Append(theme.LThemeFamily).Append(';')
            .Append("--serif:").Append(theme.LThemeSerif).Append(';')
            .Append('}');

        sheet.Append("*{box-sizing:border-box;}");
        sheet.Append("html,body{margin:0;padding:0;}");
        sheet.Append("body{background:var(--canvas);color:var(--ink);")
            .Append("font-family:var(--family);font-size:14px;line-height:1.45;")
            .Append("-webkit-print-color-adjust:exact;print-color-adjust:exact;}");
        sheet.Append(".portrait{max-width:960px;margin:0 auto;padding:34px 30px 40px 30px;}");

        sheet.Append(".crest{display:flex;align-items:center;flex-wrap:wrap;gap:14px;}");
        sheet.Append(".headword{font-size:40px;font-weight:700;margin:0;line-height:1.15;}");
        sheet.Append(".tongue{display:inline-flex;align-items:center;gap:8px;")
            .Append("background:var(--accent-soft);border-radius:12px;padding:5px 16px 5px 10px;}");
        sheet.Append(".tongue span{font-size:12px;font-weight:600;color:var(--accent);}");
        sheet.Append(".tongue i{width:12px;height:12px;border-radius:50%;")
            .Append("border:1.2px solid var(--accent);display:inline-block;}");
        sheet.Append(".star{font-size:17px;color:var(--accent);line-height:1;}");

        sheet.Append(".sound{margin:12px 0 0 12px;font-size:19px;}");
        sheet.Append(".sound b{font-weight:500;color:var(--ink);margin:0 3px;}");
        sheet.Append(".sound em{font-style:normal;color:var(--muted);}");
        sheet.Append(".sound span{font-size:14px;color:var(--muted);}");

        sheet.Append(".speech{margin:14px 0 0 12px;display:flex;flex-wrap:wrap;}");
        sheet.Append(".speech span{height:28px;line-height:28px;margin:0 7px 7px 0;padding:0 12px;")
            .Append("border-radius:14px;background:var(--accent-soft);")
            .Append("font-size:12px;font-weight:600;color:var(--accent);}");

        sheet.Append(".band{margin:39px 0 0 0;}");
        sheet.Append(".band+.band{margin-top:28px;}");
        sheet.Append(".band>h2{font-size:19px;font-weight:600;margin:0 2px 12px 2px;padding:5px 0 0 0;}");

        sheet.Append(".card{background:var(--surface);border:1px solid var(--line);")
            .Append("border-radius:12px;margin:0 0 15px 0;break-inside:avoid;page-break-inside:avoid;}");
        sheet.Append(".card>header{display:flex;align-items:center;height:55px;padding:0 22px;")
            .Append("border-bottom:1px solid var(--line);border-radius:12px 12px 0 0;}");
        sheet.Append(".rank{width:28px;height:28px;border-radius:14px;background:var(--accent-soft);")
            .Append("color:var(--accent);font-weight:600;margin-right:12px;flex:0 0 auto;")
            .Append("display:flex;align-items:center;justify-content:center;}");
        sheet.Append(".title{font-size:16px;font-weight:600;padding-right:34px;}");
        sheet.Append(".title.kind{color:var(--muted);}");
        sheet.Append(".card>section{margin:21px 30px 24px 30px;padding-right:76px;}");

        sheet.Append(".phrase{font-size:16px;font-weight:600;margin:0 0 7px 0;}");
        sheet.Append(".sense{font-size:16px;margin:0;}");

        sheet.Append(".scene{margin:18px 0 0 0;display:flex;flex-wrap:wrap;}");
        sheet.Append(".scene span{margin:0 9px 7px 0;padding:4px 10px;border-radius:11px;")
            .Append("background:var(--situation-soft);border:1px solid var(--situation-edge);")
            .Append("font-size:13px;color:var(--situation);}");

        sheet.Append(".tone{margin:10px 0 0 0;display:flex;flex-wrap:wrap;}");
        sheet.Append(".scene+.tone{margin-top:4px;}");
        sheet.Append(".tone span{margin:0 9px 7px 0;padding:4px 10px;border-radius:11px;")
            .Append("background:transparent;border:1px solid var(--line);")
            .Append("font-size:13px;color:var(--muted);}");

        sheet.Append(".bridge{margin:15px 0 0 0;display:flex;flex-wrap:wrap;align-items:center;}");
        sheet.Append(".scene+.bridge{margin-top:6px;}");
        sheet.Append(".bridge span{margin:0 9px 7px 0;padding:5px 10px;border-radius:13px;")
            .Append("background:var(--accent-soft);display:inline-flex;align-items:center;gap:7px;}");
        sheet.Append(".bridge b{font-size:13px;font-weight:600;color:var(--accent);}");
        sheet.Append(".bridge i{font-style:normal;font-size:11px;color:var(--muted);}");

        sheet.Append(".quotes{margin:16px 0 0 0;padding:1px 0 0 14px;}");
        sheet.Append(".quote{display:flex;align-items:flex-start;margin:0 0 7px 0;}");
        sheet.Append(".quote>.dot{color:var(--muted);font-size:15px;margin-right:9px;}");
        sheet.Append(".quote>.frame{color:var(--accent);font-size:15px;font-weight:700;")
            .Append("margin-right:16px;white-space:nowrap;}");
        sheet.Append(".quote>.said{font-family:var(--serif);font-size:15px;}");

        sheet.Append(".labels{margin:24px 0 0 0;display:flex;flex-wrap:wrap;}");
        sheet.Append(".labels span{margin:0 7px 7px 0;padding:4px 10px;border-radius:11px;")
            .Append("border:1px solid var(--line);font-size:12px;color:var(--muted);}");

        sheet.Append(".plates{margin:16px 0 0 14px;}");
        sheet.Append(".plate{display:inline-block;background:var(--surface);border:1px solid var(--line);")
            .Append("border-radius:9px;padding:7px;margin:0 0 7px 0;max-width:100%;")
            .Append("break-inside:avoid;page-break-inside:avoid;}");
        sheet.Append(".plate img{display:block;max-width:100%;max-height:460px;height:auto;border-radius:3px;}");
        sheet.Append(".reel{position:relative;display:block;text-decoration:none;color:inherit;}");
        sheet.Append(".reel .glyph{position:absolute;left:50%;top:50%;transform:translate(-50%,-50%);")
            .Append("width:52px;height:52px;border-radius:26px;background:rgba(23,32,51,.66);")
            .Append("color:#fff;font-size:19px;line-height:52px;text-align:center;}");
        sheet.Append(".reel .said{display:block;font-size:12px;color:var(--muted);")
            .Append("margin-top:6px;word-break:break-all;}");
        sheet.Append(".reel .blank{width:320px;height:180px;border-radius:3px;")
            .Append("background:var(--accent-soft);}");

        sheet.Append(".rows{margin:0;}");
        sheet.Append(".row{display:flex;align-items:center;gap:12px;background:var(--surface);")
            .Append("border:1px solid var(--line);border-radius:12px;padding:14px 16px;margin:0 0 8px 0;")
            .Append("break-inside:avoid;page-break-inside:avoid;}");
        sheet.Append(".row>.mark{width:32px;height:32px;border-radius:10px;background:var(--accent-soft);")
            .Append("color:var(--accent);text-align:center;line-height:32px;flex:0 0 auto;}");
        sheet.Append(".row>.body{flex:1 1 auto;min-width:0;}");
        sheet.Append(".row>.body b{display:block;font-size:15px;font-weight:600;}");
        sheet.Append(".row>.body span{display:block;font-size:12px;color:var(--muted);margin-top:3px;}");
        sheet.Append(".row>.pill{padding:3px 9px;border-radius:9px;background:var(--accent-soft);")
            .Append("font-size:11px;color:var(--accent);flex:0 0 auto;}");
        sheet.Append(".row>.speak{font-size:12px;color:var(--muted);flex:0 0 auto;}");

        sheet.Append(".note{background:var(--surface);border:1px solid var(--line);border-radius:12px;")
            .Append("padding:22px 26px;font-size:14px;line-height:21px;}");
        sheet.Append(".note>:first-child{margin-top:0;}.note>:last-child{margin-bottom:0;}");
        sheet.Append(".note p,.note ul,.note ol,.note blockquote,.note pre{margin:0 0 10px;}");
        sheet.Append(".note h3,.note h4,.note h5,.note h6{margin:14px 0 6px;font-size:15px;font-weight:600;}");
        sheet.Append(".note ul,.note ol{padding-left:22px;}.note li{margin:2px 0;}");
        sheet.Append(".note blockquote{border-left:3px solid var(--line);padding-left:12px;color:var(--muted);}");
        sheet.Append(".note code{font-family:Consolas,monospace;font-size:13px;background:var(--accent-soft);")
            .Append("padding:1px 4px;border-radius:4px;}");
        sheet.Append(".note pre{white-space:pre-wrap;background:var(--accent-soft);padding:10px 12px;border-radius:8px;}");
        sheet.Append(".note pre code{background:none;padding:0;}");
        sheet.Append(".note hr{border:0;border-top:1px solid var(--line);margin:12px 0;}");
        sheet.Append(".note a{color:var(--accent);}");

        sheet.Append("@page{size:A4;margin:12mm;}");
        sheet.Append("@media print{body{background:#fff;}.portrait{padding:0;max-width:none;}}");

        return sheet.ToString();
    }
}
