using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TPress : LPress
{
    internal string? TPressHtml { get; private set; }

    internal string? TPressPath { get; private set; }

    internal bool TPressOpened { get; private set; }

    internal LPressBoard TPressBoard { get; set; } = new TPressBoard();

    public Task LPressSave(string html, string path)
    {
        TPressHtml = html;
        TPressPath = path;
        return Task.CompletedTask;
    }

    public Task<LPressBoard> LPressBoardOpen(string html)
    {
        TPressHtml = html;
        TPressOpened = true;
        return Task.FromResult<LPressBoard>(TPressBoard);
    }
}
