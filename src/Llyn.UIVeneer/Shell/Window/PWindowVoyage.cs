using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIVeneer;

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
            ("Tenor", PTenor, PTenor.PTenorVoyageRead),
            ("Reference", PReference, PReference.PReferenceVoyageRead),
            ("Guild", PGuild, PGuild.PGuildVoyageRead),
            ("Favorite", PFavorite, PFavorite.PFavoriteVoyageRead),
            ("Phonology", PPhonology, PPhonology.PPhonologyVoyageRead),
            ("Yunjing", PYunjing, PYunjing.PYunjingVoyageRead),
            ("Xiesheng", PXiesheng, PXiesheng.PXieshengVoyageRead)
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

    internal void PVoyageRecord()
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
                "Reference" => PWindowSourceShow(station.PVoyageId),
                "Guild" => PWindowAuthorShow(station.PVoyageId),
                "Favorite" => PWindowFavoriteShow(station.PVoyageId),
                "Phonology" => PWindowInventoryShow(station.PVoyageId),
                "Yunjing" => PWindowXiaoyunShow(station.PVoyageId),
                "Xiesheng" => PWindowKindredShow(station.PVoyageId),
                _ => false,
            };
        }
        finally
        {
            _pVoyageSailing = false;
        }
    }

    internal void PVoyageRetreatRun()
    {
        PVoyageRun(_pVoyagePast, _pVoyageFuture);
    }

    internal void PVoyageAdvanceRun()
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
        bool past = _pVoyagePast.Count > 0;
        bool future = _pVoyageFuture.Count > 0;

        PCorpus.PCorpusVoyageShow(past, future);
        PRepertoire.PRepertoireVoyageShow(past, future);
        PReference.PReferenceVoyageShow(past, future);
        PFavorite.PFavoriteVoyageShow(past, future);
        PGuild.PGuildVoyageShow(past, future);
        PLibrary.PLibraryVoyageShow(past, future);
        PTaxonomy.PTaxonomyVoyageShow(past, future);
        PTenor.PTenorVoyageShow(past, future);
        PPhonology.PPhonologyVoyageShow(past, future);
        PYunjing.PYunjingVoyageShow(past, future);
        PXiesheng.PXieshengVoyageShow(past, future);
    }
}
