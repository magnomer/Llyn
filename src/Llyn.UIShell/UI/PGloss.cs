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
    private LStateValue _pGlossText;
    private bool _pGlossLanguageVisible;

    internal PGloss(ObservableCollection<PLanguageItem> catalog, LGlossDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        PGlossLanguageCatalog = catalog;
        _pGlossId = draft.LGlossDraftId;
        _pGlossLanguage = draft.LGlossDraftLanguage;
        _pGlossFlag = PEnsign.PEnsignFind(_pGlossLanguage);
        _pGlossText = draft.LGlossDraftText;
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

    public LStateValue PGlossText
    {
        get => _pGlossText;
        private set
        {
            if (_pGlossText == value)
            {
                return;
            }

            _pGlossText = value;
            PGlossRaise(nameof(PGlossText));
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

    internal void PGlossShow(LGlossDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (!string.Equals(_pGlossLanguage, draft.LGlossDraftLanguage, StringComparison.Ordinal))
        {
            _pGlossLanguage = draft.LGlossDraftLanguage;
            PGlossFlag = PEnsign.PEnsignFind(_pGlossLanguage);
            PGlossRaise(nameof(PGlossLanguage));
        }

        PGlossText = draft.LGlossDraftText;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PGlossRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
