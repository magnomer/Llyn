using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PWindow
{
    private const int PVoyageCap = 50;

    private readonly Stack<(string PVoyageMode, long PVoyageId)> _pVoyagePast = new();

    private readonly Stack<(string PVoyageMode, long PVoyageId)> _pVoyageFuture = new();

    private bool _pVoyageSailing;

    private (string PVoyageMode, FrameworkElement PVoyagePanel, Func<long> PVoyageReader)[] PVoyagePanelRead()
    {
        return
        [
            ("Library", PLibrary, PLibrary.PLibraryVoyageRead),
            ("Repertoire", PRepertoire, PRepertoire.PRepertoireVoyageRead),
            ("Taxonomy", PTaxonomy, PTaxonomy.PTaxonomyVoyageRead),
            ("Corpus", PCorpus, PCorpus.PCorpusVoyageRead),
            ("Tenor", PTenor, PTenor.PTenorVoyageRead)
        ];
    }

    private (string PVoyageMode, long PVoyageId) PVoyageStationRead()
    {
        foreach ((string mode, FrameworkElement panel, Func<long> reader) in PVoyagePanelRead())
        {
            if (panel.Visibility == Visibility.Visible)
            {
                return (mode, reader());
            }
        }

        return (string.Empty, 0);
    }

    private void PVoyageRecord()
    {
        if (_pVoyageSailing)
        {
            return;
        }

        (string PVoyageMode, long PVoyageId) station = PVoyageStationRead();
        if (station.PVoyageId == 0 || (_pVoyagePast.Count > 0 && _pVoyagePast.Peek() == station))
        {
            return;
        }

        PVoyageStationAdd(_pVoyagePast, station);
        _pVoyageFuture.Clear();
        PVoyageUpdate();
    }

    private static void PVoyageStationAdd(
        Stack<(string PVoyageMode, long PVoyageId)> trail, (string PVoyageMode, long PVoyageId) station)
    {
        if (trail.Count >= PVoyageCap)
        {
            (string PVoyageMode, long PVoyageId)[] kept = trail.Take(PVoyageCap - 1).Reverse().ToArray();
            trail.Clear();
            foreach ((string PVoyageMode, long PVoyageId) held in kept)
            {
                trail.Push(held);
            }
        }

        trail.Push(station);
    }

    private bool PVoyageStationShow((string PVoyageMode, long PVoyageId) station)
    {
        _pVoyageSailing = true;
        try
        {
            return station.PVoyageMode switch
            {
                "Library" => PWindowEntryShow(station.PVoyageId),
                "Repertoire" => PWindowSituationShow(station.PVoyageId),
                "Taxonomy" => PWindowTagShow(station.PVoyageId),
                "Corpus" => PWindowExampleShow(station.PVoyageId),
                "Tenor" => PWindowRegisterShow(station.PVoyageId),
                _ => false,
            };
        }
        finally
        {
            _pVoyageSailing = false;
        }
    }

    private void PVoyageRetreatRun()
    {
        PVoyageRun(_pVoyagePast, _pVoyageFuture);
    }

    private void PVoyageAdvanceRun()
    {
        PVoyageRun(_pVoyageFuture, _pVoyagePast);
    }

    private void PVoyageRun(
        Stack<(string PVoyageMode, long PVoyageId)> source, Stack<(string PVoyageMode, long PVoyageId)> target)
    {
        if (source.Count == 0)
        {
            return;
        }

        (string PVoyageMode, long PVoyageId) current = PVoyageStationRead();
        if (!PVoyageStationShow(source.Peek()))
        {
            return;
        }

        source.Pop();
        if (current.PVoyageId != 0)
        {
            PVoyageStationAdd(target, current);
        }

        PVoyageUpdate();
    }

    private void PVoyageHandle(object sender, RoutedEventArgs e)
    {
        if (sender == PVoyageBackward)
        {
            PVoyageRetreatRun();
            return;
        }

        PVoyageAdvanceRun();
    }

    private void PVoyageKeyHandle(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.System || Keyboard.Modifiers != ModifierKeys.Alt)
        {
            return;
        }

        if (e.SystemKey == Key.Left)
        {
            PVoyageRetreatRun();
            e.Handled = true;
        }
        else if (e.SystemKey == Key.Right)
        {
            PVoyageAdvanceRun();
            e.Handled = true;
        }
    }

    private void PVoyageMouseHandle(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.XButton1)
        {
            PVoyageRetreatRun();
            e.Handled = true;
        }
        else if (e.ChangedButton == MouseButton.XButton2)
        {
            PVoyageAdvanceRun();
            e.Handled = true;
        }
    }

    private void PVoyageUpdate()
    {
        PVoyageBackward.IsEnabled = _pVoyagePast.Count > 0;
        PVoyageForward.IsEnabled = _pVoyageFuture.Count > 0;
    }
}
