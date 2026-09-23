using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PSCustomsItem : INotifyPropertyChanged
{
    private LMarkupMode _psCustomsItemMode;
    private long _psCustomsItemTarget;
    private string _psCustomsItemLoss = string.Empty;

    internal PSCustomsItem(int index, LMarkupEntry entry, IReadOnlyList<LEntry> candidates)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(candidates);

        PSCustomsItemIndex = index;
        PSCustomsItemHeadword = entry.LMarkupEntryName;
        PSCustomsItemLanguage = entry.LMarkupEntryLanguage;
        PSCustomsItemCandidate = candidates;
        _psCustomsItemMode = candidates.Count == 1 ? LMarkupMode.LMarkupModeMerge : LMarkupMode.LMarkupModeNew;
        _psCustomsItemTarget = candidates.Count == 1 ? candidates[0].LEntryId : 0;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public int PSCustomsItemIndex { get; }

    public string PSCustomsItemNumber => (PSCustomsItemIndex + 1).ToString(CultureInfo.CurrentCulture);

    public string PSCustomsItemLanguage { get; }

    public IReadOnlyList<LEntry> PSCustomsItemCandidate { get; }

    public string PSCustomsItemHeadword { get; }

    public LMarkupMode PSCustomsItemMode
    {
        get => _psCustomsItemMode;
        set
        {
            if (_psCustomsItemMode == value)
            {
                return;
            }

            _psCustomsItemMode = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PSCustomsItemMode)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PSCustomsItemTargeted)));
        }
    }

    public long PSCustomsItemTarget
    {
        get => _psCustomsItemTarget;
        set
        {
            if (_psCustomsItemTarget == value)
            {
                return;
            }

            _psCustomsItemTarget = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PSCustomsItemTarget)));
        }
    }

    public bool PSCustomsItemTargeted => _psCustomsItemMode != LMarkupMode.LMarkupModeNew;

    public string PSCustomsItemLoss
    {
        get => _psCustomsItemLoss;
        set
        {
            string text = value ?? string.Empty;
            if (string.Equals(_psCustomsItemLoss, text, StringComparison.Ordinal))
            {
                return;
            }

            _psCustomsItemLoss = text;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PSCustomsItemLoss)));
        }
    }

    public bool PSCustomsItemReady =>
        _psCustomsItemMode == LMarkupMode.LMarkupModeNew || _psCustomsItemTarget > 0;
}
