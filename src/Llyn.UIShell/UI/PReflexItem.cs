using System;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PReflexItem : INotifyPropertyChanged
{
    private readonly PWindow _pReflexItemHost;
    private string _pReflexItemLanguage;
    private string _pReflexItemKind;
    private string _pReflexItemText;
    private string _pReflexItemNote;
    private bool _pReflexItemMain;
    private bool _pReflexItemLead;

    internal PReflexItem(
        PWindow host, long id, string language, string kind, string text, string note, bool main, bool respelled)
    {
        _pReflexItemHost = host;
        PReflexItemId = id;
        _pReflexItemLanguage = language;
        _pReflexItemKind = kind;
        _pReflexItemText = text;
        _pReflexItemNote = note;
        _pReflexItemMain = main;
        PReflexItemRespelled = respelled;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long PReflexItemId { get; }

    public bool PReflexItemRespelled { get; }

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
        set => PReflexItemSet(ref _pReflexItemText, value, nameof(PReflexItemText));
    }

    public string PReflexItemNote
    {
        get => _pReflexItemNote;
        set => PReflexItemSet(ref _pReflexItemNote, value, nameof(PReflexItemNote));
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
        }
    }

    public string PReflexItemHead
    {
        get => _pReflexItemLead ? _pReflexItemLanguage : string.Empty;
        set => PReflexItemLanguage = value;
    }

    public string PReflexItemLabel =>
        _pReflexItemLead ? PReflexLabelFormat(_pReflexItemHost, _pReflexItemLanguage) : string.Empty;

    public string PReflexItemTag => PReflexLabelFormat(_pReflexItemHost, _pReflexItemKind);

    internal static PReflexItem PReflexItemCreate(PWindow host, LReflexDraft draft, PRespelling respelling)
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
            draft.LReflexDraftNote,
            draft.LReflexDraftMain,
            respelling.PRespellingShown);
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

        return name.Length == 0 ? string.Empty : host.PLocalizationTextFind(string.Concat("Reflex.", name)) ?? name;
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
