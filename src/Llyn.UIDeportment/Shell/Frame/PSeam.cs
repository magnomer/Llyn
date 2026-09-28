using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public sealed class PSeam : Thumb
{
    private double[] _pSeamWidth = [];

    private double[] _pSeamRoom = [];

    private double _pSeamOrigin;

    public PSeam()
    {
        Cursor = Cursors.SizeWE;
        DragStarted += PSeamStartRefine;
        DragDelta += PSeamDragRefine;
        DragCompleted += PSeamFinishRefine;
    }

    internal PLayout? PSeamLayout { get; set; }

    private void PSeamStartRefine(object sender, DragStartedEventArgs e)
    {
        if (Parent is not Grid host)
        {
            return;
        }

        PSeamColumnRefine(host);
        host.UpdateLayout();

        int count = host.ColumnDefinitions.Count;
        _pSeamWidth = new double[count];
        _pSeamRoom = new double[count];

        for (int index = 0; index < count; index++)
        {
            _pSeamWidth[index] = host.ColumnDefinitions[index].ActualWidth;
        }

        for (int index = 0; index < count; index++)
        {
            _pSeamRoom[index] = Math.Min(PSeamRoomRead(host, index), _pSeamWidth[index]);
        }

        host.UpdateLayout();

        _pSeamOrigin = Mouse.GetPosition(host).X;
    }

    private void PSeamDragRefine(object sender, DragDeltaEventArgs e)
    {
        if (Parent is not Grid host || _pSeamWidth.Length != host.ColumnDefinitions.Count)
        {
            return;
        }

        int last = host.ColumnDefinitions.Count - 1;
        int seam = Grid.GetColumn(this);
        if (seam < 1 || seam > last)
        {
            return;
        }

        for (int index = 0; index < last; index++)
        {
            host.ColumnDefinitions[index].Width = new GridLength(_pSeamWidth[index]);
        }

        double shift = Mouse.GetPosition(host).X - _pSeamOrigin;
        int grow = shift > 0 ? seam - 1 : seam;
        int pay = shift > 0 ? last : 0;

        shift = Math.Min(Math.Abs(shift), _pSeamWidth[pay] - _pSeamRoom[pay]);
        if (shift <= 0)
        {
            PSeamLayout?.PLayoutLinkRefine(host);
            return;
        }

        if (grow != last)
        {
            host.ColumnDefinitions[grow].Width = new GridLength(_pSeamWidth[grow] + shift);
        }

        if (pay != last)
        {
            host.ColumnDefinitions[pay].Width = new GridLength(_pSeamWidth[pay] - shift);
        }

        PSeamLayout?.PLayoutLinkRefine(host);
    }

    private void PSeamFinishRefine(object sender, DragCompletedEventArgs e)
    {
        if (Parent is Grid host && _pSeamWidth.Length == host.ColumnDefinitions.Count)
        {
            PSeamLayout?.PLayoutSave(host);
        }
    }

    private static double PSeamRoomRead(Grid host, int column)
    {
        double room = 0;

        foreach (UIElement child in host.Children)
        {
            if (child is PSeam ||
                child.Visibility != Visibility.Visible ||
                Grid.GetColumn(child) != column ||
                Grid.GetColumnSpan(child) != 1)
            {
                continue;
            }

            double height = child.RenderSize.Height > 0 ? child.RenderSize.Height : double.PositiveInfinity;

            child.Measure(new Size(0, height));
            room = Math.Max(room, child.DesiredSize.Width);
            child.InvalidateMeasure();
        }

        return room;
    }

    private static void PSeamColumnRefine(Grid host)
    {
        int last = host.ColumnDefinitions.Count - 1;
        for (int index = 0; index < last; index++)
        {
            ColumnDefinition column = host.ColumnDefinitions[index];
            column.Width = new GridLength(column.ActualWidth);
        }

        host.ColumnDefinitions[last].Width = new GridLength(1, GridUnitType.Star);
    }
}
