using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public sealed class QSeam : Thumb
{
    private double[] _qSeamWidth = [];

    private double[] _qSeamRoom = [];

    private double _qSeamOrigin;

    public QSeam()
    {
        Cursor = Cursors.SizeWE;
        DragStarted += QSeamStartRefine;
        DragDelta += QSeamDragRefine;
        DragCompleted += QSeamFinishRefine;
    }

    internal event Action<Grid>? QSeamDragNotice;

    internal event Action<Grid>? QSeamReleaseNotice;

    private void QSeamStartRefine(object sender, DragStartedEventArgs e)
    {
        if (Parent is not Grid host)
        {
            return;
        }

        QSeamColumnRefine(host);
        host.UpdateLayout();

        int count = host.ColumnDefinitions.Count;
        _qSeamWidth = new double[count];
        _qSeamRoom = new double[count];

        for (int index = 0; index < count; index++)
        {
            _qSeamWidth[index] = host.ColumnDefinitions[index].ActualWidth;
        }

        for (int index = 0; index < count; index++)
        {
            _qSeamRoom[index] = Math.Min(QSeamRoomRead(host, index), _qSeamWidth[index]);
        }

        host.UpdateLayout();

        _qSeamOrigin = Mouse.GetPosition(host).X;
    }

    private void QSeamDragRefine(object sender, DragDeltaEventArgs e)
    {
        if (Parent is not Grid host || _qSeamWidth.Length != host.ColumnDefinitions.Count)
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
            host.ColumnDefinitions[index].Width = new GridLength(_qSeamWidth[index]);
        }

        double shift = Mouse.GetPosition(host).X - _qSeamOrigin;
        int grow = shift > 0 ? seam - 1 : seam;
        int pay = shift > 0 ? last : 0;

        shift = Math.Min(Math.Abs(shift), _qSeamWidth[pay] - _qSeamRoom[pay]);
        if (shift <= 0)
        {
            QSeamDragNotice?.Invoke(host);
            return;
        }

        if (grow != last)
        {
            host.ColumnDefinitions[grow].Width = new GridLength(_qSeamWidth[grow] + shift);
        }

        if (pay != last)
        {
            host.ColumnDefinitions[pay].Width = new GridLength(_qSeamWidth[pay] - shift);
        }

        QSeamDragNotice?.Invoke(host);
    }

    private void QSeamFinishRefine(object sender, DragCompletedEventArgs e)
    {
        if (Parent is Grid host && _qSeamWidth.Length == host.ColumnDefinitions.Count)
        {
            QSeamReleaseNotice?.Invoke(host);
        }
    }

    private static double QSeamRoomRead(Grid host, int column)
    {
        double room = 0;

        foreach (UIElement child in host.Children)
        {
            if (child is QSeam ||
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

    private static void QSeamColumnRefine(Grid host)
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
