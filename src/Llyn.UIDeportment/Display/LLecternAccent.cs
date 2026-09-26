using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LLecternAccent
{
    private readonly LDisplaySound _lLecternAccentDisplay;

    private readonly ObservableCollection<LAccentItem> _lLecternAccentRow = [];

    private LWindow _lLecternAccentWindow = null!;

    private UIElement _lLecternAccentSurface = null!;

    private ColumnDefinition _lLecternAccentLead = null!;

    private Image _lLecternAccentFlag = null!;

    private TextBlock _lLecternAccentLabel = null!;

    private TextBlock _lLecternAccentOpener = null!;

    private TextBlock _lLecternAccentPronunciation = null!;

    private TextBlock _lLecternAccentCloser = null!;

    private DependencyObject _lLecternAccentContour = null!;

    private DependencyProperty _lLecternAccentTonal = null!;

    public LLecternAccent(LDisplaySound display)
    {
        ArgumentNullException.ThrowIfNull(display);

        _lLecternAccentDisplay = display;
    }

    public void LLecternAccentAttach(
        LWindow window,
        UIElement surface,
        ColumnDefinition lead,
        Image flag,
        TextBlock label,
        TextBlock opener,
        TextBlock pronunciation,
        TextBlock closer,
        ItemsControl accents,
        DependencyObject contour,
        DependencyProperty tonal)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(lead);
        ArgumentNullException.ThrowIfNull(flag);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(opener);
        ArgumentNullException.ThrowIfNull(pronunciation);
        ArgumentNullException.ThrowIfNull(closer);
        ArgumentNullException.ThrowIfNull(accents);
        ArgumentNullException.ThrowIfNull(contour);
        ArgumentNullException.ThrowIfNull(tonal);

        _lLecternAccentWindow = window;
        _lLecternAccentSurface = surface;
        _lLecternAccentLead = lead;
        _lLecternAccentFlag = flag;
        _lLecternAccentLabel = label;
        _lLecternAccentOpener = opener;
        _lLecternAccentPronunciation = pronunciation;
        _lLecternAccentCloser = closer;
        _lLecternAccentContour = contour;
        _lLecternAccentTonal = tonal;
        accents.ItemsSource = _lLecternAccentRow;
        PLookItem.PLookItemAttach(accents, LAccentItem.LAccentItemApply);
    }

    public void LLecternAccentShow()
    {
        LLecternAccentShow(
            _lLecternAccentDisplay.LDisplayShown!.LEntryDraftLanguage,
            _lLecternAccentDisplay.LDisplayFlaggedCheck(),
            _lLecternAccentDisplay.LDisplayShown!.LEntryDraftPronunciation);
    }

    public void LLecternAccentClear()
    {
        _lLecternAccentSurface.Visibility = Visibility.Collapsed;
        _lLecternAccentLead.SharedSizeGroup = null;
        _lLecternAccentRow.Clear();
        LLecternPrimaryShow();
    }

    private void LLecternAccentShow(string language, bool flagged, LPronunciationDraft? primary)
    {
        LRespellingMark respelling = LRespellingMark.LRespellingMarkRead(_lLecternAccentWindow, language);

        _lLecternAccentOpener.Text = respelling.LRespellingMarkOpener;
        _lLecternAccentCloser.Text = respelling.LRespellingMarkCloser;
        _lLecternAccentContour.SetValue(_lLecternAccentTonal, _lLecternAccentDisplay.LDisplayTonalCheck());
        LLecternSurfaceShow(primary is null ? string.Empty : respelling.LRespellingMarkResolve(primary));

        _lLecternAccentRow.Clear();
        foreach (LPronunciationDraft spoken in _lLecternAccentDisplay.LDisplayAccentRead())
        {
            _lLecternAccentRow.Add(LAccentItem.LAccentItemCreate(language, flagged, spoken, respelling));
        }

        LLecternPrimaryShow();
        _ = LLecternFlagLoad(language, flagged);
    }

    private void LLecternSurfaceShow(string text)
    {
        _lLecternAccentPronunciation.Text = text;
        _lLecternAccentSurface.Visibility = text.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
        _lLecternAccentLead.SharedSizeGroup = text.Length == 0 ? null : "PReadingLabel";
    }

    private void LLecternPrimaryShow()
    {
        LEntryDraft? draft = _lLecternAccentDisplay.LDisplayShown;
        string variety = draft?.LEntryDraftPronunciation?.LPronunciationDraftVariety ?? string.Empty;
        _lLecternAccentFlag.Source = draft is null
            ? null
            : LAccentItem.LAccentFlagFind(
                draft.LEntryDraftLanguage, _lLecternAccentDisplay.LDisplayFlaggedCheck(), variety);
        _lLecternAccentLabel.Text = _lLecternAccentFlag.Source is null
            ? LAccentItem.LAccentLabelFormat(variety)
            : string.Empty;
    }

    private async Task LLecternFlagLoad(string language, bool flagged)
    {
        if (!flagged)
        {
            return;
        }

        List<string> varieties = _lLecternAccentRow.Select(static row => row.LAccentItemVariety).ToList();
        varieties.Add(
            _lLecternAccentDisplay.LDisplayShown?.LEntryDraftPronunciation?.LPronunciationDraftVariety
            ?? string.Empty);

        try
        {
            await LEnsignImage.LEnsignVarietyLoad(
                _lLecternAccentWindow, language, varieties.Where(static variety => variety.Length > 0));
        }
        catch (Exception)
        {
            return;
        }

        if (!_lLecternAccentDisplay.LDisplayFlaggedCheck(language))
        {
            return;
        }

        foreach (LAccentItem row in _lLecternAccentRow)
        {
            row.LAccentFlagUpdate(language, flagged);
        }

        LLecternPrimaryShow();
    }
}
