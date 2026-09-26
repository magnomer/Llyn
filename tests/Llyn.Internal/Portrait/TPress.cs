using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TPress : LPress
{
    internal string? TPressHtml { get; private set; }

    internal string? TPressPath { get; private set; }

    internal LPressTicket? TPressTicket { get; private set; }

    public Task LPressSave(string html, string path)
    {
        TPressHtml = html;
        TPressPath = path;
        return Task.CompletedTask;
    }

    public Task LPressPrint(string html, LPressTicket ticket)
    {
        TPressHtml = html;
        TPressTicket = ticket;
        return Task.CompletedTask;
    }
}
