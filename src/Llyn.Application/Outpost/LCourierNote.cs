using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LCourierNote
{
    private const string LCourierTagPrefix = "llyn/";

    private readonly LOutpost _lCourierNoteOutpost;
    private readonly LLivery _lCourierNoteLivery;

    public LCourierNote(LOutpost outpost, LLivery livery)
    {
        ArgumentNullException.ThrowIfNull(outpost);
        ArgumentNullException.ThrowIfNull(livery);
        _lCourierNoteOutpost = outpost;
        _lCourierNoteLivery = livery;
    }

    public static Func<long, string> LCourierNoteBuild(LLivery livery, string stamp, IReadOnlyList<LEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(stamp);
        ArgumentNullException.ThrowIfNull(entries);

        HashSet<long> sending = [.. entries.Select(static entry => entry.LEntryId)];
        return entryId => sending.Contains(entryId)
            ? livery.LLiveryIdFormat("llyn:entry:" + stamp + ":" + entryId.ToString(CultureInfo.InvariantCulture))
            : string.Empty;
    }

    public async Task<bool> LCourierEntrySend(
        int port,
        string token,
        string id,
        string folder,
        string style,
        LEntry entry,
        Func<long, LLiveryPage?> page,
        Func<long, string> note,
        Func<string, string, string, string> link,
        Func<string, string> lookup,
        Dictionary<string, string> digests,
        CancellationToken cancellation)
    {
        LLiveryPage read = page(entry.LEntryId)
            ?? throw new InvalidOperationException("The entry no longer stands in the workspace.");

        LLiveryNote rendered = _lCourierNoteLivery.LLiveryFormat(read, style, note, link, lookup);
        LOutpostNote outpost = new(id, folder, entry.LEntryHeadword, rendered.LLiveryNoteBody);
        return await LCourierNoteSend(
            port, token, outpost, LCourierTagRead(read.LLiveryPageDraft), rendered.LLiveryNoteParcel, digests,
            cancellation)
            .ConfigureAwait(false);
    }

    public async Task<bool> LCourierNoteSend(
        int port,
        string token,
        LOutpostNote note,
        IReadOnlyList<string> tags,
        IReadOnlyList<LParcel> parcels,
        Dictionary<string, string> digests,
        CancellationToken cancellation)
    {
        string digest = _lCourierNoteLivery.LLiveryDigestFormat(note, tags, parcels);
        if (digests.TryGetValue(note.LOutpostNoteId, out string? held)
            && string.Equals(held, digest, StringComparison.Ordinal))
        {
            string? body = await _lCourierNoteOutpost.LOutpostNoteRead(
                port, token, note.LOutpostNoteId, note.LOutpostNoteFolder, note.LOutpostNoteTitle, cancellation)
                .ConfigureAwait(false);
            if (string.Equals(body, note.LOutpostNoteBody, StringComparison.Ordinal))
            {
                await _lCourierNoteOutpost.LOutpostTagSave(port, token, note.LOutpostNoteId, tags, cancellation)
                    .ConfigureAwait(false);
                return false;
            }
        }

        foreach (LParcel parcel in parcels)
        {
            await _lCourierNoteOutpost.LOutpostParcelSave(port, token, parcel, cancellation)
                .ConfigureAwait(false);
        }

        await _lCourierNoteOutpost.LOutpostNoteSave(port, token, note, cancellation).ConfigureAwait(false);
        await _lCourierNoteOutpost.LOutpostTagSave(port, token, note.LOutpostNoteId, tags, cancellation)
            .ConfigureAwait(false);
        digests[note.LOutpostNoteId] = digest;
        return true;
    }

    private static IReadOnlyList<string> LCourierTagRead(LEntryDraft draft)
    {
        SortedSet<string> tags = new(StringComparer.Ordinal);
        if (!string.IsNullOrWhiteSpace(draft.LEntryDraftLanguage))
        {
            tags.Add(LCourierTagPrefix + draft.LEntryDraftLanguage.Trim());
        }

        Stack<LCardDraft> cards = new(draft.LEntryDraftMeanings.Concat(draft.LEntryDraftCollocations));
        while (cards.Count > 0)
        {
            LCardDraft card = cards.Pop();
            foreach (LTagDraft tag in card.LCardDraftTag)
            {
                if (!string.IsNullOrWhiteSpace(tag.LTagDraftText))
                {
                    tags.Add(LCourierTagPrefix + tag.LTagDraftText.Trim());
                }
            }

            foreach (LCardDraft child in card.LCardDraftChild)
            {
                cards.Push(child);
            }
        }

        return [.. tags];
    }
}
