using System;
using System.ComponentModel;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal sealed class PContext : INotifyPropertyChanged
{
    private string _pContextId;
    private string _pContextText;
    private bool _pContextUnreadable;
    private string _pContextOrderText;

    internal PContext()
        : this(LStateValue.LStateValueUnspecified, string.Empty)
    {
    }

    internal PContext(LStateValue text, string id)
    {
        ArgumentNullException.ThrowIfNull(text);

        _pContextId = id;
        _pContextText = text.LStateValueShow();
        _pContextUnreadable = text.LStateValueState == LState.LStateUnknown;
        _pContextOrderText = string.Empty;
    }

    public string PContextId
    {
        get => _pContextId;
        private set
        {
            if (string.Equals(_pContextId, value, StringComparison.Ordinal))
            {
                return;
            }

            _pContextId = value;
            PContextRaise(nameof(PContextId));
        }
    }

    public string PContextText
    {
        get => _pContextText;
        set
        {
            if (string.Equals(_pContextText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pContextText = value;
            PContextUnreadable = false;
            PContextRaise(nameof(PContextText));
        }
    }

    public bool PContextUnreadable
    {
        get => _pContextUnreadable;
        private set
        {
            if (_pContextUnreadable == value)
            {
                return;
            }

            _pContextUnreadable = value;
            PContextRaise(nameof(PContextUnreadable));
        }
    }

    public string PContextOrderText
    {
        get => _pContextOrderText;
        set
        {
            if (string.Equals(_pContextOrderText, value, StringComparison.Ordinal))
            {
                return;
            }

            _pContextOrderText = value;
            PContextRaise(nameof(PContextOrderText));
        }
    }

    internal LStateValue PContextTextRead()
    {
        return _pContextUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pContextText);
    }

    internal void PContextIdentityApply()
    {
        if (_pContextId.Length != 0 || PContextTextRead().LStateValueEmpty)
        {
            return;
        }

        PContextId = LEngine.LEngineIdentityCreate();
    }

    internal void PContextClear()
    {
        PContextText = string.Empty;
        PContextUnreadable = false;
        PContextId = string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PContextRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
