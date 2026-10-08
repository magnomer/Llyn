using System;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public sealed class QLecternLead
{
    private readonly UIElement _qLecternLeadSection;

    private readonly ColumnDefinition _qLecternLeadColumn;

    public QLecternLead(UIElement section, ColumnDefinition column)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(column);

        _qLecternLeadSection = section;
        _qLecternLeadColumn = column;
    }

    public void QLecternLeadRefine(bool shown)
    {
        _qLecternLeadSection.Visibility = QLook.QLookVisibleRead(shown);
        _qLecternLeadColumn.SharedSizeGroup = shown ? "PReadingLabel" : null;
    }
}
