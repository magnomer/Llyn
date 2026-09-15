using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public sealed class PLayout
{
    private readonly LEngine _lEngine;

    private readonly List<(string PLayoutTab, Grid PLayoutHost, GridLength PLayoutLeft, GridLength PLayoutMiddle)>
        _pLayoutList = [];

    private Grid? _pLayoutRecent;

    public PLayout(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lEngine = engine;
    }

    internal void PLayoutAttach(Grid host, string tab)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentException.ThrowIfNullOrWhiteSpace(tab);

        int last = host.ColumnDefinitions.Count - 1;

        GridLength left = last > 0 ? host.ColumnDefinitions[0].Width : GridLength.Auto;
        GridLength middle = last > 1 ? host.ColumnDefinitions[1].Width : GridLength.Auto;

        _pLayoutList.Add((tab, host, left, middle));

        foreach (UIElement child in host.Children)
        {
            if (child is PSeam seam)
            {
                seam.PSeamLayout = this;
            }
        }
    }

    internal void PLayoutRestore()
    {
        LSettings settings = _lEngine.LEngineSettingsRead();
        IReadOnlyList<LLayout> layout = settings.LSettingsLayout ?? [];

        foreach ((string tab, Grid host, _, _) in _pLayoutList)
        {
            foreach (LLayout record in layout)
            {
                if (!string.Equals(record.LLayoutTab, tab, StringComparison.Ordinal))
                {
                    continue;
                }

                PLayoutColumnApply(host, 0, record.LLayoutLeft);
                PLayoutColumnApply(host, 1, record.LLayoutMiddle);
                break;
            }
        }

        if (!settings.LSettingsLinked)
        {
            return;
        }

        double? left = null;
        double? middle = null;

        foreach ((_, Grid host, _, _) in _pLayoutList)
        {
            int last = host.ColumnDefinitions.Count - 1;

            left ??= last > 0 ? PLayoutColumnRead(host.ColumnDefinitions[0]) : null;
            middle ??= last > 1 ? PLayoutColumnRead(host.ColumnDefinitions[1]) : null;
        }

        foreach ((_, Grid host, _, _) in _pLayoutList)
        {
            PLayoutColumnApply(host, 0, left);
            PLayoutColumnApply(host, 1, middle);
        }
    }

    internal void PLayoutPropagate(Grid source)
    {
        ArgumentNullException.ThrowIfNull(source);

        _pLayoutRecent = source;

        if (!_lEngine.LEngineSettingsRead().LSettingsLinked)
        {
            return;
        }

        int last = source.ColumnDefinitions.Count - 1;

        foreach ((_, Grid host, _, _) in _pLayoutList)
        {
            if (ReferenceEquals(host, source))
            {
                continue;
            }

            for (int index = 0; index < last; index++)
            {
                PLayoutColumnApply(host, index, PLayoutColumnRead(source.ColumnDefinitions[index]));
            }
        }
    }

    internal void PLayoutSave(Grid source)
    {
        ArgumentNullException.ThrowIfNull(source);

        _pLayoutRecent = source;

        List<LLayout> list = [];

        foreach ((string tab, Grid host, _, _) in _pLayoutList)
        {
            if (ReferenceEquals(host, source) || _lEngine.LEngineSettingsRead().LSettingsLinked)
            {
                list.Add(PLayoutRecordRead(tab, host));
            }
        }

        _lEngine.LEngineLayoutSave(list);
    }

    internal void PLayoutSync()
    {
        Grid? source = _pLayoutRecent ?? (_pLayoutList.Count > 0 ? _pLayoutList[0].PLayoutHost : null);
        if (source is null)
        {
            return;
        }

        PLayoutPropagate(source);
        PLayoutSave(source);
    }

    internal void PLayoutReset()
    {
        foreach ((_, Grid host, GridLength left, GridLength middle) in _pLayoutList)
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

        _pLayoutRecent = null;
    }

    private static LLayout PLayoutRecordRead(string tab, Grid host)
    {
        int last = host.ColumnDefinitions.Count - 1;

        double? left = last > 0 ? PLayoutColumnRead(host.ColumnDefinitions[0]) : null;
        double? middle = last > 1 ? PLayoutColumnRead(host.ColumnDefinitions[1]) : null;

        return new LLayout(tab, left, middle);
    }

    private static double? PLayoutColumnRead(ColumnDefinition column)
    {
        if (column.Width.IsAbsolute)
        {
            return column.Width.Value;
        }

        return column.ActualWidth > 0 ? column.ActualWidth : null;
    }

    private static void PLayoutColumnApply(Grid host, int index, double? width)
    {
        int last = host.ColumnDefinitions.Count - 1;

        if (width is not double value || index >= last)
        {
            return;
        }

        host.ColumnDefinitions[index].Width = new GridLength(value);
    }
}
