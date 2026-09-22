using System;
using System.Collections.Generic;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PReflexItem : INotifyPropertyChanged
{
    private readonly PWindow _pReflexItemHost;
    private string _pReflexItemLanguage;
    private string _pReflexItemKind;
    private string _pReflexItemText;
    private string _pReflexItemRomanization;
    private string _pReflexItemMeaning;
    private string _pReflexItemNote;
    private string _pReflexItemRegion;
    private bool _pReflexItemMain;
    private bool _pReflexItemLead;
    private bool _pReflexItemHidden;
    private IReadOnlyList<long> _pReflexItemAnchors = [];
    private string _pReflexItemAnchor = string.Empty;
    private bool _pReflexItemAnchorable;

    internal PReflexItem(
        PWindow host,
        long id,
        string language,
        string kind,
        string text,
        string romanization,
        string meaning,
        string note,
        bool main,
        bool respelled,
        string region,
        bool phonemic,
        bool folded,
        IReadOnlyList<long> anchors)
    {
        _pReflexItemHost = host;
        _pReflexItemAnchors = anchors;
        PReflexItemId = id;
        _pReflexItemLanguage = language;
        _pReflexItemKind = kind;
        _pReflexItemText = text;
        _pReflexItemRomanization = romanization;
        _pReflexItemMeaning = meaning;
        _pReflexItemNote = note;
        _pReflexItemRegion = region;
        _pReflexItemMain = main;
        PReflexItemRespelled = respelled;
        PReflexItemPhonemic = phonemic;
        PReflexItemFolded = folded;
        _pReflexItemHidden = folded;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long PReflexItemId { get; }

    public bool PReflexItemRespelled { get; }

    internal bool PReflexItemMatch(bool respelled, bool phonemic, bool folded)
    {
        return PReflexItemRespelled == respelled && PReflexItemPhonemic == phonemic && PReflexItemFolded == folded;
    }

    public bool PReflexItemPhonemic { get; }

    public bool PReflexItemFolded { get; }

    public string PReflexItemTone { get; set; } = string.Empty;

    public string PReflexItemOpener => PReflexItemPhonemic ? "/" : string.Empty;

    public string PReflexItemCloser => PReflexItemPhonemic ? "/" : string.Empty;

    public string PReflexItemLanguage
    {
        get => _pReflexItemLanguage;
        set
        {
            if (PReflexItemSet(ref _pReflexItemLanguage, value, nameof(PReflexItemLanguage)))
            {
                PReflexItemRaise(nameof(PReflexItemHead));
                PReflexItemRaise(nameof(PReflexItemLabel));
            }
        }
    }

    public string PReflexItemKind
    {
        get => _pReflexItemKind;
        set
        {
            if (PReflexItemSet(ref _pReflexItemKind, value, nameof(PReflexItemKind)))
            {
                PReflexItemRaise(nameof(PReflexItemTag));
            }
        }
    }

    public string PReflexItemText
    {
        get => _pReflexItemText;
        set
        {
            PReflexItemSet(ref _pReflexItemText, value, nameof(PReflexItemText));
        }
    }

    public string PReflexItemRomanization
    {
        get => _pReflexItemRomanization;
        set => PReflexItemSet(ref _pReflexItemRomanization, value, nameof(PReflexItemRomanization));
    }

    public string PReflexItemMeaning
    {
        get => _pReflexItemMeaning;
        set => PReflexItemSet(ref _pReflexItemMeaning, value, nameof(PReflexItemMeaning));
    }

    public string PReflexItemNote
    {
        get => _pReflexItemNote;
        set => PReflexItemSet(ref _pReflexItemNote, value, nameof(PReflexItemNote));
    }

    public string PReflexItemRegion
    {
        get => _pReflexItemRegion;
        set
        {
            if (PReflexItemSet(ref _pReflexItemRegion, value, nameof(PReflexItemRegion)))
            {
                PReflexItemRaise(nameof(PReflexItemArea));
            }
        }
    }

    public bool PReflexItemMain
    {
        get => _pReflexItemMain;
        set
        {
            if (_pReflexItemMain == value)
            {
                return;
            }

            _pReflexItemMain = value;
            PReflexItemRaise(nameof(PReflexItemMain));
        }
    }

    public bool PReflexItemLead
    {
        get => _pReflexItemLead;
        set
        {
            if (_pReflexItemLead == value)
            {
                return;
            }

            _pReflexItemLead = value;
            PReflexItemRaise(nameof(PReflexItemLead));
            PReflexItemRaise(nameof(PReflexItemHead));
            PReflexItemRaise(nameof(PReflexItemLabel));
            PReflexItemRaise(nameof(PReflexItemArea));
        }
    }

    public bool PReflexItemHidden
    {
        get => _pReflexItemHidden;
        set
        {
            if (_pReflexItemHidden == value)
            {
                return;
            }

            _pReflexItemHidden = value;
            PReflexItemRaise(nameof(PReflexItemHidden));
        }
    }

    public IReadOnlyList<long> PReflexItemAnchors
    {
        get => _pReflexItemAnchors;
        set
        {
            IReadOnlyList<long> anchors = value ?? [];
            if (_pReflexItemHost.PWindowDeportment.LWindowAnchorMatch(_pReflexItemAnchors, anchors))
            {
                return;
            }

            _pReflexItemAnchors = anchors;
            PReflexItemRaise(nameof(PReflexItemAnchors));
        }
    }

    public string PReflexItemAnchor
    {
        get => _pReflexItemAnchor;
        set => PReflexItemSet(ref _pReflexItemAnchor, value, nameof(PReflexItemAnchor));
    }

    public bool PReflexItemAnchorable
    {
        get => _pReflexItemAnchorable;
        set
        {
            if (_pReflexItemAnchorable == value)
            {
                return;
            }

            _pReflexItemAnchorable = value;
            PReflexItemRaise(nameof(PReflexItemAnchorable));
        }
    }

    public string PReflexItemHead
    {
        get => _pReflexItemLead ? _pReflexItemLanguage : string.Empty;
        set => PReflexItemLanguage = value;
    }

    public string PReflexItemLabel =>
        _pReflexItemLead ? PReflexLabelFormat(_pReflexItemHost, _pReflexItemLanguage) : string.Empty;

    public string PReflexItemArea => _pReflexItemLead ? _pReflexItemRegion : string.Empty;

    public string PReflexItemTag => PReflexLabelFormat(_pReflexItemHost, _pReflexItemKind);

    internal static PReflexItem PReflexItemCreate(
        PWindow host, LReflexDraft draft, PRespelling respelling, bool phonemic, bool folded)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(respelling);

        return new PReflexItem(
            host,
            draft.LReflexDraftId,
            draft.LReflexDraftLanguage,
            draft.LReflexDraftKind,
            PReflexTextRead(draft, respelling),
            draft.LReflexDraftRomanization,
            draft.LReflexDraftMeaning,
            draft.LReflexDraftNote,
            draft.LReflexDraftMain,
            respelling.PRespellingShown,
            draft.LReflexDraftRegion,
            phonemic,
            folded,
            draft.LReflexDraftAnchors)
        {
            PReflexItemTone = draft.LReflexDraftAnatomy.LAnatomyToneIpa,
        };
    }

    internal static string PReflexTextRead(LReflexDraft draft, PRespelling respelling)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(respelling);

        return respelling.PRespellingTextRead(draft.LReflexDraftText, draft.LReflexDraftRespelling);
    }

    internal static string PReflexLabelFormat(PWindow host, string name)
    {
        ArgumentNullException.ThrowIfNull(host);

        return name.Length == 0
            ? string.Empty
            : PLocalizationCatalog.PLocalizationTextFind(string.Concat("Reflex.", name)) ?? name;
    }

    private bool PReflexItemSet(ref string field, string? value, string name)
    {
        string text = value ?? string.Empty;
        if (string.Equals(field, text, StringComparison.Ordinal))
        {
            return false;
        }

        field = text;
        PReflexItemRaise(name);
        return true;
    }

    private void PReflexItemRaise(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
