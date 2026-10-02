using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;
using Llyn.UIDeportment.Capsule;

namespace Llyn.UIDeportment;

public sealed class QLayout
{
    private readonly QPosture _qLayoutPosture;

    private readonly List<(string QLayoutTab, Grid QLayoutHost, GridLength QLayoutLeft, GridLength QLayoutMiddle)>
        _qLayoutList = [];

    private Grid? _qLayoutRecent;

    private FrameworkElement _qLayoutSettings = null!;

    public QLayout(QPosture posture)
    {
        ArgumentNullException.ThrowIfNull(posture);
        _qLayoutPosture = posture;
        _qLayoutPosture.QPostureCleared += QLayoutResetRefine;
        _qLayoutPosture.QPostureLinkedChanged += QLayoutLinkedSync;
    }

    private ToggleButton QLayoutLinked => QContract.QContractFind<ToggleButton>(_qLayoutSettings, "PLayoutLinked");

    internal void QLayoutIntroduce(FrameworkElement settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _qLayoutSettings = settings;
        QLayoutLinked.Click += QLayoutLinkedObserve;
    }

    internal void QLayoutLinkedRefine()
    {
        QLayoutLinked.IsChecked = _qLayoutPosture.QPostureRead().LCapsuleContentLinked;
    }

    private void QLayoutLinkedObserve(object sender, RoutedEventArgs e)
    {
        _qLayoutPosture.QPostureLinkedSave(QLook.QLookCheckedRead(QLayoutLinked.IsChecked));
    }

    internal void QLayoutAttach(Grid host, string tab)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentException.ThrowIfNullOrWhiteSpace(tab);

        int last = host.ColumnDefinitions.Count - 1;

        GridLength left = last > 0 ? host.ColumnDefinitions[0].Width : GridLength.Auto;
        GridLength middle = last > 1 ? host.ColumnDefinitions[1].Width : GridLength.Auto;

        _qLayoutList.Add((tab, host, left, middle));

        foreach (UIElement child in host.Children)
        {
            if (child is QSeam seam)
            {
                seam.QSeamDragNotice += QLayoutLinkRefine;
                seam.QSeamReleaseNotice += QLayoutSave;
            }
        }
    }

    internal void QLayoutRefine()
    {
        foreach ((string tab, Grid host, _, _) in _qLayoutList)
        {
            if (_qLayoutPosture.QPostureColumnRead(tab) is LCapsuleColumn record)
            {
                QLayoutColumnRefine(host, 0, record.LCapsuleColumnLeft);
                QLayoutColumnRefine(host, 1, record.LCapsuleColumnMiddle);
            }
        }

        if (!_qLayoutPosture.QPostureRead().LCapsuleContentLinked)
        {
            return;
        }

        double? left = null;
        double? middle = null;

        foreach ((_, Grid host, _, _) in _qLayoutList)
        {
            int last = host.ColumnDefinitions.Count - 1;

            left ??= last > 0 ? QLayoutColumnRead(host.ColumnDefinitions[0]) : null;
            middle ??= last > 1 ? QLayoutColumnRead(host.ColumnDefinitions[1]) : null;
        }

        foreach ((_, Grid host, _, _) in _qLayoutList)
        {
            QLayoutColumnRefine(host, 0, left);
            QLayoutColumnRefine(host, 1, middle);
        }
    }

    internal void QLayoutLinkRefine(Grid? dragged)
    {
        if (QLayoutSourceRead(dragged) is not Grid source)
        {
            return;
        }

        _qLayoutRecent = source;

        if (!_qLayoutPosture.QPostureRead().LCapsuleContentLinked)
        {
            return;
        }

        int last = source.ColumnDefinitions.Count - 1;

        foreach ((_, Grid host, _, _) in _qLayoutList)
        {
            if (ReferenceEquals(host, source))
            {
                continue;
            }

            for (int index = 0; index < last; index++)
            {
                QLayoutColumnRefine(host, index, QLayoutColumnRead(source.ColumnDefinitions[index]));
            }
        }
    }

    internal void QLayoutSave(Grid? dragged)
    {
        if (QLayoutSourceRead(dragged) is not Grid source)
        {
            return;
        }

        _qLayoutRecent = source;

        List<LCapsuleColumn> list = [];

        foreach ((string tab, Grid host, _, _) in _qLayoutList)
        {
            if (!ReferenceEquals(host, source))
            {
                if (!_qLayoutPosture.QPostureRead().LCapsuleContentLinked)
                {
                    continue;
                }
            }

            list.Add(QLayoutRecordRead(tab, host));
        }

        _qLayoutPosture.QPostureLayoutSave(list);
    }

    private void QLayoutLinkedSync()
    {
        QLayoutLinkRefine(null);
        QLayoutSave(null);
    }

    internal void QLayoutResetRefine()
    {
        foreach ((_, Grid host, GridLength left, GridLength middle) in _qLayoutList)
        {
            int last = host.ColumnDefinitions.Count - 1;

            if (last > 0)
            {
                host.ColumnDefinitions[0].Width = left;
            }

            if (last > 1)
            {
                host.ColumnDefinitions[1].Width = middle;
            }
        }

        _qLayoutRecent = null;
    }

    private Grid? QLayoutSourceRead(Grid? dragged)
    {
        return dragged ?? _qLayoutRecent ?? (_qLayoutList.Count > 0 ? _qLayoutList[0].QLayoutHost : null);
    }

    private static LCapsuleColumn QLayoutRecordRead(string tab, Grid host)
    {
        int last = host.ColumnDefinitions.Count - 1;

        double? left = last > 0 ? QLayoutColumnRead(host.ColumnDefinitions[0]) : null;
        double? middle = last > 1 ? QLayoutColumnRead(host.ColumnDefinitions[1]) : null;

        return new LCapsuleColumn(tab, left, middle);
    }

    private static double? QLayoutColumnRead(ColumnDefinition column)
    {
        if (column.Width.IsAbsolute)
        {
            return column.Width.Value;
        }

        return column.ActualWidth > 0 ? column.ActualWidth : null;
    }

    private static void QLayoutColumnRefine(Grid host, int index, double? width)
    {
        int last = host.ColumnDefinitions.Count - 1;

        if (width is not double value || index >= last)
        {
            return;
        }

        host.ColumnDefinitions[index].Width = new GridLength(value);
    }
}
