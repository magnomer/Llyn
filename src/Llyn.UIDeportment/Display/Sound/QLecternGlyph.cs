using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternGlyph
{
    private readonly CDisplaySound _qLecternGlyphArea;

    private readonly ObservableCollection<QGlyphItem> _qLecternSoundGlyph = [];

    private readonly QLecternLead _qLecternGlyphLead;

    private readonly TextBlock _qLecternSoundHeading;

    private readonly ItemsControl _qLecternSoundStrip;

    public QLecternGlyph(FrameworkElement surface, CDisplaySound area)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);

        _qLecternGlyphArea = area;
        _qLecternGlyphLead = new QLecternLead(
            QContract.QContractFind<Border>(surface, "PDisplayGlyphSection"),
            QContract.QContractFind<ColumnDefinition>(surface, "PDisplayGlyphLead"));
        _qLecternSoundHeading = QContract.QContractFind<TextBlock>(surface, "PDisplayGlyphLabel");
        _qLecternSoundStrip = QContract.QContractFind<ItemsControl>(surface, "PDisplayGlyph");

        _qLecternSoundStrip.ItemsSource = _qLecternSoundGlyph;
        QLookItem.QLookItemAttach(_qLecternSoundStrip, QGlyphItem.QGlyphItemRefine);
        _qLecternSoundStrip.CommandBindings.Add(new CommandBinding(
            QGlyphCommand.QGlyphCommandEntry, QLecternGlyphObserve));
    }

    public void QLecternGlyphRefine()
    {
        CLecternGlyph glyph = _qLecternGlyphArea.CDisplayGlyphRead();
        _qLecternSoundGlyph.Clear();
        foreach (CGlyphCell cell in glyph.CLecternGlyphCells)
        {
            _qLecternSoundGlyph.Add(
                new QGlyphItem(cell.CGlyphCellText, cell.CGlyphCellLanguage, cell.CGlyphCellLinked));
        }

        _qLecternSoundHeading.Text = glyph.CLecternGlyphShown
            ? QTranscriptionItem.QTranscriptionLabelRefine(glyph.CLecternGlyphKey, glyph.CLecternGlyphName)
            : string.Empty;
        QFontFace.QFontGlyphRefine(_qLecternSoundStrip.Resources, glyph.CLecternGlyphFont);
        _qLecternGlyphLead.QLecternLeadRefine(glyph.CLecternGlyphShown);
    }

    private void QLecternGlyphObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QGlyphItem { QGlyphItemLinked: true } item)
        {
            _qLecternGlyphArea.CDisplayGlyphOpen(item.QGlyphItemText, item.QGlyphItemLanguage);
        }
    }
}
