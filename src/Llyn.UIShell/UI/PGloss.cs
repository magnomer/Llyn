using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PGloss : INotifyPropertyChanged
{
    private readonly long _pGlossId;
    private string _pGlossLanguage;
    private ImageSource? _pGlossFlag;
    private string _pGlossText;
    private bool _pGlossUnknown;
    private bool _pGlossLanguageVisible;

    internal PGloss(ObservableCollection<PLanguageItem> catalog, LGlossDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        PGlossLanguageCatalog = catalog;
        _pGlossId = draft.LGlossDraftId;
        _pGlossLanguage = draft.LGlossDraftLanguage;
        _pGlossFlag = PEnsign.PEnsignFind(_pGlossLanguage);
        _pGlossText = draft.LGlossDraftText.LStateValueShow();
        _pGlossUnknown = draft.LGlossDraftText.LStateValueState == LState.LStateUnknown;
    }

    public ObservableCollection<PLanguageItem> PGlossLanguageCatalog { get; }

    public long PGlossId => _pGlossId;

    public string PGlossLanguage
    {
        get => _pGlossLanguage;
        set
        {
            if (value is null)
            {
                return;
            }

            PGlossLanguageVisible = false;
            if (string.Equals(_pGlossLanguage, value, StringComparison.Ordinal))
            {
                return;
            }

            _pGlossLanguage = value;
            PGlossFlag = PEnsign.PEnsignFind(value);
            PGlossRaise(nameof(PGlossLanguage));
        }
    }

    public ImageSource? PGlossFlag
    {
        get => _pGlossFlag;
        private set
        {
            if (ReferenceEquals(_pGlossFlag, value))
            {
                return;
            }

            _pGlossFlag = value;
            PGlossRaise(nameof(PGlossFlag));
        }
    }

    public string PGlossText
    {
        get => _pGlossText;
        set
        {
            if (string.Equals(_pGlossText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pGlossText = value;
            PGlossUnknown = false;
            PGlossRaise(nameof(PGlossText));
        }
    }

    public bool PGlossUnknown
    {
        get => _pGlossUnknown;
        private set
        {
            if (_pGlossUnknown == value)
            {
                return;
            }

            _pGlossUnknown = value;
            PGlossRaise(nameof(PGlossUnknown));
        }
    }

    public bool PGlossLanguageVisible
    {
        get => _pGlossLanguageVisible;
        set
        {
            if (_pGlossLanguageVisible == value)
            {
                return;
            }

            _pGlossLanguageVisible = value;
            PGlossRaise(nameof(PGlossLanguageVisible));
        }
    }

    internal LStateWritten PGlossTextRead()
    {
        return new LStateWritten(_pGlossText, _pGlossUnknown);
    }

    internal void PGlossShow(LGlossDraft draft, Func<string, bool> pending)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(pending);

        if (!string.Equals(_pGlossLanguage, draft.LGlossDraftLanguage, StringComparison.Ordinal))
        {
            _pGlossLanguage = draft.LGlossDraftLanguage;
            PGlossFlag = PEnsign.PEnsignFind(_pGlossLanguage);
            PGlossRaise(nameof(PGlossLanguage));
        }

        if (!pending(nameof(PGlossText)) && !PGlossTextRead().LStateWrittenMatch(draft.LGlossDraftText))
        {
            _pGlossText = draft.LGlossDraftText.LStateValueShow();
            PGlossRaise(nameof(PGlossText));
            PGlossUnknown = draft.LGlossDraftText.LStateValueState == LState.LStateUnknown;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PGlossRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
