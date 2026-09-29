using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LReferenceClerk
{
    private readonly LReferenceVault _lReferenceClerkReferences;
    private readonly LAuthorVault _lReferenceClerkAuthors;

    public LReferenceClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lReferenceClerkReferences = rig.LRigReferences;
        _lReferenceClerkAuthors = rig.LRigAuthors;
    }

    public static LReference LReferenceClerkBlank =>
        new(
            0,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified);

    public LReference LReferenceClerkCreate(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        return _lReferenceClerkReferences.LReferenceCreate(reference);
    }

    public LReference LReferenceClerkCreate(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        LStateValue named = LStateValue.LStateValueRead(new LStateWritten(title.Trim(), false));
        return _lReferenceClerkReferences.LReferenceCreate(LReferenceClerkBlank with { LReferenceTitle = named });
    }

    public long? LReferenceClerkResolve(string title, long held)
    {
        ArgumentNullException.ThrowIfNull(title);

        string typed = LCatalog.LCatalogTextNormalize(title);
        if (typed.Length == 0)
        {
            return 0;
        }

        IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits = _lReferenceClerkAuthors.LAuthorReferenceRead();
        long? titled = null;
        long? signed = null;
        foreach (LReference reference in _lReferenceClerkReferences.LReferenceAllRead())
        {
            long id = reference.LReferenceId;
            credits.TryGetValue(id, out IReadOnlyList<LAuthor>? credited);
            bool named = LCatalog.LCatalogTextNormalize(reference.LReferenceTitle.LStateValueShow()) == typed;
            bool bylined = LCatalog.LCatalogTextNormalize(reference.LReferenceBylineRead(credited ?? [])) == typed;
            if (id == held && (named || bylined))
            {
                return id;
            }

            if (named)
            {
                titled = Math.Min(titled ?? id, id);
            }

            if (bylined)
            {
                signed = Math.Min(signed ?? id, id);
            }
        }

        return titled ?? signed;
    }

    public LReference? LReferenceClerkRead(long id)
    {
        return _lReferenceClerkReferences.LReferenceRead(id);
    }

    public IReadOnlyList<LReference> LReferenceClerkRead()
    {
        return _lReferenceClerkReferences.LReferenceAllRead();
    }

    public LPortraitPage? LReferenceClerkRead(long id, LPortraitLegend legend)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(legend);

        LReference? reference = _lReferenceClerkReferences.LReferenceRead(id);
        if (reference is null)
        {
            return null;
        }

        IReadOnlyList<LAuthor> credits = _lReferenceClerkAuthors.LAuthorReferenceRead(id);
        int cites = _lReferenceClerkReferences.LReferenceUsageRead(id).Count;
        return LReferencePageRead(reference, credits, cites, legend);
    }

    public IReadOnlyList<LCatalogReference> LReferenceClerkFind(string query, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(query);
        query = query.Trim();

        IReadOnlyList<LReference> read = _lReferenceClerkReferences.LReferenceAllRead();
        IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits = _lReferenceClerkAuthors.LAuthorReferenceRead();
        IReadOnlyDictionary<long, int> usage = _lReferenceClerkReferences.LReferenceUsageRead();

        List<LCatalogReference> rows = [];
        foreach (LReference reference in read)
        {
            credits.TryGetValue(reference.LReferenceId, out IReadOnlyList<LAuthor>? credited);
            usage.TryGetValue(reference.LReferenceId, out int counted);

            LCatalogReference row = LCatalogReference.LCatalogReferenceCreate(reference, credited, counted);
            if (row.LCatalogReferenceMatch(query))
            {
                rows.Add(row);
            }
        }

        return LCatalogReference.LCatalogReferenceSort(rows, order);
    }

    public LReferenceOffer LReferenceCitationFind(string text, LDraft? draft, long card, long sentence)
    {
        ArgumentNullException.ThrowIfNull(text);

        string word = text.Trim();
        long? held = draft?.LDraftExampleRead(card, sentence)?.LExampleDraftReference.LStateAnchorShown;
        List<LReferenceRow> rows = [];
        foreach (LCatalogReference row in LReferenceCitationFind(word, held))
        {
            (string lead, string mark, string tail) = LCatalog.LCatalogMarkFind(row.LCatalogReferenceByline, word);
            rows.Add(new LReferenceRow(
                row.LCatalogReferenceStored.LReferenceId,
                lead,
                mark,
                tail,
                LCatalog.LCatalogUsageFormat(row.LCatalogReferenceUsage)));
            if (rows.Count == LCatalog.LCatalogOfferLimit)
            {
                break;
            }
        }

        return new LReferenceOffer(text, rows, rows.Count > 0);
    }

    private IReadOnlyList<LCatalogReference> LReferenceCitationFind(string word, long? held)
    {
        ArgumentNullException.ThrowIfNull(word);

        string text = word.Trim();
        if (text.Length == 0)
        {
            return [];
        }

        IReadOnlyList<LCatalogReference> found = LReferenceClerkFind(text, LCatalogOrder.LCatalogOrderUsage);
        foreach (LCatalogReference row in found)
        {
            if (row.LCatalogReferenceStored.LReferenceId == held
                && string.Equals(row.LCatalogReferenceByline, text, StringComparison.Ordinal))
            {
                return [];
            }
        }

        return found;
    }

    public IReadOnlyList<LCatalogReference> LReferenceOeuvreFind(
        long? author,
        string query,
        LCatalogFilter kind,
        LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(kind);
        string written = query.Trim();

        IReadOnlyList<LReference> references = _lReferenceClerkReferences.LReferenceAllRead();
        IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits = _lReferenceClerkAuthors.LAuthorReferenceRead();
        IReadOnlyDictionary<long, int> usage = _lReferenceClerkReferences.LReferenceUsageRead();

        List<LCatalogReference> rows = [];
        foreach (LReference reference in references)
        {
            credits.TryGetValue(reference.LReferenceId, out IReadOnlyList<LAuthor>? credited);
            if (!LAuthorClerk.LOeuvreMatch(author, credited))
            {
                continue;
            }

            if (!kind.LCatalogFilterMatch(LReference.LReferenceKindFormat(reference.LReferenceKind)))
            {
                continue;
            }

            usage.TryGetValue(reference.LReferenceId, out int counted);
            LCatalogReference row = LCatalogReference.LCatalogReferenceCreate(reference, credited, counted);
            if (row.LCatalogReferenceMatch(written))
            {
                rows.Add(row);
            }
        }

        return LCatalogReference.LCatalogReferenceSort(rows, order);
    }

    public IReadOnlyDictionary<long, string> LCitationRead()
    {
        IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits = _lReferenceClerkAuthors.LAuthorReferenceRead();

        Dictionary<long, string> named = [];
        foreach (LReference reference in _lReferenceClerkReferences.LReferenceAllRead())
        {
            credits.TryGetValue(reference.LReferenceId, out IReadOnlyList<LAuthor>? credited);
            named[reference.LReferenceId] = reference.LReferenceBylineRead(credited ?? []);
        }

        return named;
    }

    public IReadOnlyDictionary<long, string> LCitationRead(LEntryDraft shown)
    {
        ArgumentNullException.ThrowIfNull(shown);

        IReadOnlyDictionary<long, string> named = LCitationRead();
        Dictionary<long, string> lines = [];
        foreach (long id in shown.LEntryDraftCitations)
        {
            lines[id] = LCitationFormat(named, id);
        }

        return lines;
    }

    public string LCitationRead(LDraft? draft)
    {
        return LCitationFormat(LCitationRead(), draft?.LDraftExample?.LExampleSource.LStateAnchorShow() ?? 0);
    }

    internal static string LCitationFormat(IReadOnlyDictionary<long, string> named, long id)
    {
        if (id == 0)
        {
            return string.Empty;
        }

        return named.TryGetValue(id, out string? name)
            ? name
            : id.ToString(CultureInfo.InvariantCulture);
    }

    public void LReferenceClerkUpdate(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        _lReferenceClerkReferences.LReferenceUpdate(reference);
    }

    public void LReferenceClerkDelete(long id, bool detach)
    {
        _lReferenceClerkReferences.LReferenceDelete(id, detach);
    }

    public static bool LReferenceClerkMatch(LReference one, LReference other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        return one.LReferenceTitle == other.LReferenceTitle
            && one.LReferenceYear == other.LReferenceYear
            && one.LReferenceKind == other.LReferenceKind
            && one.LReferenceNote == other.LReferenceNote
            && one.LReferenceUrl == other.LReferenceUrl
            && one.LReferenceAuthorState == other.LReferenceAuthorState;
    }

    public static LColophon LReferenceColophonRead(LDraft draft, string tally)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LReference reference = draft.LDraftReference
            ?? throw new InvalidOperationException("The draft holds no reference.");
        return reference.LReferenceColophonRead(draft.LDraftAuthor, tally, LLocalization.QLocalizationTextRead);
    }

    public static LImprint LReferenceImprintRead(LDraft? draft)
    {
        if (draft is null)
        {
            return LImprint.LImprintCreate(LReferenceClerkBlank);
        }

        return LImprint.LImprintCreate(
            draft.LDraftReference ?? throw new InvalidOperationException("The draft holds no reference."));
    }

    public static IReadOnlyList<(string LReferenceKindTag, string LReferenceKindKey)> LReferenceMenuRead()
    {
        List<(string LReferenceKindTag, string LReferenceKindKey)> menu = new(LReference.LReferenceKindMenu.Count);
        foreach (LReferenceKind kind in LReference.LReferenceKindMenu)
        {
            menu.Add((LReference.LReferenceKindFormat(kind), LReference.LReferenceKindResolve(kind)));
        }

        return menu;
    }

    public static LReferenceKind? LReferenceKindRead(LDraft? draft, string? tag)
    {
        if (tag is null)
        {
            return null;
        }

        LReferenceKind kind = LReference.LReferenceKindParse(tag);
        return draft?.LDraftReference?.LReferenceKindMatch(kind) is true ? null : kind;
    }

    public static LPortraitPage LReferencePageRead(
        LReference reference, IReadOnlyList<LAuthor> credits, int count, LPortraitLegend legend)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(credits);
        ArgumentNullException.ThrowIfNull(legend);

        string mark = legend.LPortraitLegendUnknown;
        List<string> chips = [];

        if (reference.LReferenceKind != LReferenceKind.LReferenceKindUnspecified)
        {
            chips.Add(legend.LPortraitKindFormat(reference.LReferenceKind));
        }

        chips.Add(legend.LPortraitTallyFormat(count));

        List<LPortraitSection> sections = [];

        if (credits.Count > 0)
        {
            string[] names = new string[credits.Count];
            for (int index = 0; index < credits.Count; index++)
            {
                names[index] = credits[index].LAuthorName;
            }

            sections.Add(LPortraitSection.LPortraitSectionCreate(
                legend.LPortraitLegendAuthor, string.Join(", ", names)));
        }
        else if (reference.LReferenceAuthorState.LStateMarkState == LState.LStateUnknown)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(legend.LPortraitLegendAuthor, mark));
        }

        LPortraitSection.LPortraitSectionAdd(sections, legend.LPortraitLegendYear, reference.LReferenceYear, mark);
        LPortraitSection.LPortraitSectionAdd(sections, legend.LPortraitLegendUrl, reference.LReferenceUrl, mark);
        LPortraitSection.LPortraitSectionAdd(sections, legend.LPortraitLegendNote, reference.LReferenceNote, mark);

        return new LPortraitPage(
            LPortraitText.LPortraitTitleRead(reference.LReferenceTitle, legend.LPortraitLegendUntitled, mark),
            string.Empty,
            chips,
            sections);
    }
}
