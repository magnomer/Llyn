using System;
using System.Globalization;

namespace Llyn.UIShell;

public partial class PScreen
{
    private string PScreenPaperBuild(Uri address)
    {
        string from = ((long)PScreenFrom.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        string until = (PScreenUntil is TimeSpan span && span > PScreenFrom
            ? (long)span.TotalSeconds
            : 0).ToString(CultureInfo.InvariantCulture);
        string playing = PScreenPlaying ? "1" : "0";
        string level = PScreenVolume.ToString("0.###", CultureInfo.InvariantCulture);
        string? film = PScreenFilmRead(address);

        if (film is null)
        {
            return PScreenPaperFormat(
                "<video id=\"reel\" src=\"" + PScreenTextFormat(address.AbsoluteUri) + "\" playsinline></video>\n"
                + "<script>\n"
                + "var START = " + from + ";\n"
                + "var END = " + until + ";\n"
                + "var reel = document.getElementById('reel');\n"
                + "reel.volume = " + level + ";\n"
                + "reel.addEventListener('loadedmetadata', function () { reel.currentTime = START; });\n"
                + "reel.addEventListener('timeupdate', function () {\n"
                + "    if (END > START && reel.currentTime >= END) { reel.currentTime = START; }\n"
                + "});\n"
                + "reel.addEventListener('ended', function () { reel.currentTime = START; reel.play(); });\n"
                + "window.chrome.webview.addEventListener('message', function (notice) {\n"
                + "    var order = String(notice.data);\n"
                + "    if (order.indexOf('volume:') === 0) { reel.volume = parseFloat(order.slice(7)); return; }\n"
                + "    if (order === 'play') { reel.play(); } else { reel.pause(); }\n"
                + "});\n"
                + "if (" + playing + ") { reel.play(); }\n"
                + "</script>");
        }

        return PScreenPaperFormat(
            "<div id=\"reel\"></div>\n"
            + "<script src=\"https://www.youtube.com/iframe_api\"></script>\n"
            + "<script>\n"
            + "var START = " + from + ";\n"
            + "var END = " + until + ";\n"
            + "var AUTO = " + playing + ";\n"
            + "var LEVEL = " + level + ";\n"
            + "var player = null;\n"
            + "function onYouTubeIframeAPIReady() {\n"
            + "    var settings = {\n"
            + "        start: START,\n"
            + "        autoplay: AUTO,\n"
            + "        rel: 0,\n"
            + "        modestbranding: 1,\n"
            + "        playsinline: 1,\n"
            + "        origin: 'https://llyn.video'\n"
            + "    };\n"
            + "    if (END > START) { settings.end = END; }\n"
            + "    player = new YT.Player('reel', {\n"
            + "        videoId: '" + PScreenTextFormat(film) + "',\n"
            + "        playerVars: settings,\n"
            + "        events: {\n"
            + "            onReady: function () {\n"
            + "                player.setVolume(LEVEL * 100);\n"
            + "                if (LEVEL > 0) { player.unMute(); } else { player.mute(); }\n"
            + "                if (AUTO) { player.playVideo(); }\n"
            + "            },\n"
            + "            onStateChange: function (notice) {\n"
            + "                if (notice.data === YT.PlayerState.ENDED) {\n"
            + "                    player.seekTo(START, true);\n"
            + "                    player.playVideo();\n"
            + "                }\n"
            + "            }\n"
            + "        }\n"
            + "    });\n"
            + "}\n"
            + "window.chrome.webview.addEventListener('message', function (notice) {\n"
            + "    if (!player) { return; }\n"
            + "    var order = String(notice.data);\n"
            + "    if (order.indexOf('volume:') === 0) {\n"
            + "        LEVEL = parseFloat(order.slice(7));\n"
            + "        player.setVolume(LEVEL * 100);\n"
            + "        if (LEVEL > 0) { player.unMute(); } else { player.mute(); }\n"
            + "        return;\n"
            + "    }\n"
            + "    if (order === 'play') {\n"
            + "        var moment = player.getCurrentTime();\n"
            + "        if (moment < START || (END > START && moment >= END)) { player.seekTo(START, true); }\n"
            + "        player.playVideo();\n"
            + "    } else {\n"
            + "        player.pauseVideo();\n"
            + "    }\n"
            + "});\n"
            + "</script>");
    }

    private static string PScreenPaperFormat(string body)
    {
        return "<!doctype html>\n<html>\n<head>\n<meta charset=\"utf-8\">\n<style>\n"
            + "html, body { margin: 0; height: 100%; background: #000; overflow: hidden; }\n"
            + "#reel, iframe, video { width: 100%; height: 100%; border: 0; display: block; }\n"
            + "</style>\n</head>\n<body>\n" + body + "\n</body>\n</html>";
    }

    private static string PScreenTextFormat(string text)
    {
        return text
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("'", "\'", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal);
    }
}
