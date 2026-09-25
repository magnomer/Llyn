using System;
using System.Collections.Generic;
using System.Windows;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    private void PNavigationHandle(object sender, RoutedEventArgs e)
    {
        _lNavigation.LNavigationSelect(sender);
    }

    private LTab[] PNavigationTabRead()
    {
        return
        [
            new LTab("Input", PNavigationInput, PInput),
            new LTab("Library", PNavigationLibrary, PLibrary)
            {
                LTabLeave = PLibrary.PLibraryLeaveConfirm,
                LTabScribe = PLibrary.PLibraryScribeRestore,
                LTabStation = PLibrary.PLibraryVoyageRead,
                LTabVoyage = PLibrary.PLibraryVoyageShow,
                LTabArrival = PLibrary.PIndexEntryShow
            },
            new LTab("Phonology", PNavigationPhonology, PPhonology)
            {
                LTabLeave = PPhonology.PPhonologyLeaveConfirm,
                LTabScribe = PPhonology.PPhonologyScribeRestore,
                LTabStation = PPhonology.PPhonologyVoyageRead,
                LTabVoyage = PPhonology.PPhonologyVoyageShow,
                LTabArrival = PPhonology.PInventoryEntryShow
            },
            new LTab("Xiesheng", PNavigationXiesheng, PXiesheng)
            {
                LTabAllowed = PXiesheng.PXieshengCheck,
                LTabLeave = PXiesheng.PXieshengLeaveConfirm,
                LTabScribe = PXiesheng.PXieshengScribeRestore,
                LTabStation = PXiesheng.PXieshengVoyageRead,
                LTabVoyage = PXiesheng.PXieshengVoyageShow,
                LTabArrival = PXiesheng.PKindredEntryShow
            },
            new LTab("Yunjing", PNavigationYunjing, PYunjing)
            {
                LTabAllowed = PYunjing.PYunjingCheck,
                LTabLeave = PYunjing.PYunjingLeaveConfirm,
                LTabScribe = PYunjing.PYunjingScribeRestore,
                LTabStation = PYunjing.PYunjingVoyageRead,
                LTabVoyage = PYunjing.PYunjingVoyageShow,
                LTabArrival = PYunjing.PXiaoyunEntryShow
            },
            new LTab("Taxonomy", PNavigationTaxonomy, PTaxonomy)
            {
                LTabLeave = PTaxonomy.PTaxonomyLeaveConfirm,
                LTabScribe = PTaxonomy.PTaxonomyScribeRestore,
                LTabStation = PTaxonomy.PTaxonomyVoyageRead,
                LTabVoyage = PTaxonomy.PTaxonomyVoyageShow,
                LTabArrival = PTaxonomy.PDirectoryTagShow
            },
            new LTab("Tenor", PNavigationTenor, PTenor)
            {
                LTabLeave = PTenor.PTenorLeaveConfirm,
                LTabScribe = PTenor.PTenorScribeRestore,
                LTabStation = PTenor.PTenorVoyageRead,
                LTabVoyage = PTenor.PTenorVoyageShow,
                LTabArrival = PTenor.PGamutRegisterShow
            },
            new LTab("Repertoire", PNavigationRepertoire, PRepertoire)
            {
                LTabLeave = PRepertoire.PRepertoireLeaveConfirm,
                LTabScribe = PRepertoire.PRepertoireScribeRestore,
                LTabStation = PRepertoire.PRepertoireVoyageRead,
                LTabVoyage = PRepertoire.PRepertoireVoyageShow,
                LTabArrival = PRepertoire.PAtlasSituationShow
            },
            new LTab("Corpus", PNavigationCorpus, PCorpus)
            {
                LTabLeave = PCorpus.PCorpusLeaveConfirm,
                LTabScribe = PCorpus.PCorpusScribeRestore,
                LTabStation = PCorpus.PCorpusVoyageRead,
                LTabVoyage = PCorpus.PCorpusVoyageShow,
                LTabArrival = PCorpus.PAnthologyExampleShow
            },
            new LTab("Reference", PNavigationSource, PReference)
            {
                LTabLeave = PReference.PReferenceLeaveConfirm,
                LTabScribe = PReference.PReferenceScribeRestore,
                LTabStation = PReference.PReferenceVoyageRead,
                LTabVoyage = PReference.PReferenceVoyageShow,
                LTabArrival = PReference.PShelfSourceShow
            },
            new LTab("Guild", PNavigationGuild, PGuild)
            {
                LTabLeave = PGuild.PGuildLeaveConfirm,
                LTabScribe = PGuild.PGuildScribeRestore,
                LTabStation = PGuild.PGuildVoyageRead,
                LTabVoyage = PGuild.PGuildVoyageShow,
                LTabArrival = PGuild.PRollAuthorShow
            },
            new LTab("Favorite", PNavigationFavorite, PFavorite)
            {
                LTabLeave = PFavorite.PFavoriteLeaveConfirm,
                LTabScribe = PFavorite.PFavoriteScribeRestore,
                LTabStation = PFavorite.PFavoriteVoyageRead,
                LTabVoyage = PFavorite.PFavoriteVoyageShow,
                LTabArrival = PFavorite.PRosterEntryShow
            },
            new LTab("Duplex", PNavigationDuplex, PDuplex),
            new LTab("Settings", PNavigationSettings, PSettings)
        ];
    }

    internal void PVoyageRecord()
    {
        _lNavigation.LNavigationVoyage.LVoyageRecord();
    }

    internal void PVoyageRetreatRun()
    {
        _lNavigation.LNavigationVoyage.LVoyageRetreat();
    }

    internal void PVoyageAdvanceRun()
    {
        _lNavigation.LNavigationVoyage.LVoyageAdvance();
    }

    internal bool PWindowEntryShow(long id)
    {
        PMentionMenuHide();
        return _lNavigation.LNavigationShow(PNavigationLibrary, id);
    }

    internal bool PWindowSituationShow(long id)
    {
        return _lNavigation.LNavigationShow(PNavigationRepertoire, id);
    }

    internal bool PWindowTagShow(long id)
    {
        return _lNavigation.LNavigationShow(PNavigationTaxonomy, id);
    }

    internal bool PWindowExampleShow(long id)
    {
        return _lNavigation.LNavigationShow(PNavigationCorpus, id);
    }

    internal bool PWindowRegisterShow(long id)
    {
        return _lNavigation.LNavigationShow(PNavigationTenor, id);
    }

    internal bool PWindowSourceShow(long id)
    {
        return _lNavigation.LNavigationShow(PNavigationSource, id);
    }

    internal bool PWindowAuthorShow(long id)
    {
        return _lNavigation.LNavigationShow(PNavigationGuild, id);
    }

    internal bool PWindowFavoriteShow(long id)
    {
        return _lNavigation.LNavigationShow(PNavigationFavorite, id);
    }

    internal bool PWindowInventoryShow(long id)
    {
        return _lNavigation.LNavigationShow(PNavigationPhonology, id);
    }

    internal bool PWindowXiaoyunShow(long id)
    {
        return _lNavigation.LNavigationShow(PNavigationYunjing, id);
    }

    internal bool PWindowKindredShow(long id)
    {
        return _lNavigation.LNavigationShow(PNavigationXiesheng, id);
    }

    internal void PWindowDiweiShow(string language, string kind, string key)
    {
        _lNavigation.LNavigationShow(PNavigationYunjing, () => PYunjing.PYunjingDiweiShow(language, kind, key));
    }

    internal void PWindowStemShow(string language, string? key)
    {
        _lNavigation.LNavigationShow(PNavigationXiesheng, () => PXiesheng.PXieshengStemShow(language, key));
    }

    internal void PWindowMentionHandle(PMention anchor, LMentionResult result)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(result);

        PMentionMenuHide();

        if (result.LMentionResultStored is LMention stored)
        {
            if (!stored.LMentionLinked)
            {
                return;
            }

            PWindowEntryShow(stored.LMentionEntryId);
            if (stored.LMentionSensed)
            {
                PLibrary.PDisplay.PDisplayCardScroll(stored.LMentionSenseId);
            }

            return;
        }

        if (result.LMentionResultSingle)
        {
            PWindowEntryShow(result.LMentionResultFirst);
            return;
        }

        if (result.LMentionResultMany)
        {
            PMentionMenuShow(anchor, anchor.PMentionPieceRead(result.LMentionResultOffset), result);
        }
    }

    internal void PWindowSenseShow(FrameworkElement anchor, Rect place, long entryId, Action<long> chosen)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(chosen);

        IReadOnlyList<LMeaning> meanings;
        try
        {
            meanings = _lWindow.LWindowMeaningRead(entryId);
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Mention.FindFailed", exception);
            return;
        }

        PMentionMenuShow(anchor, place, entryId, meanings, chosen);
    }

    internal void PWindowProspectShow(
        FrameworkElement anchor, Rect place, string word, string language, Action<long> chosen)
    {
        PInput.PEditor.PProspectShow(anchor, place, word, language, chosen);
    }

    private void PMentionLeaveHandle(object? sender, EventArgs e)
    {
        PMentionMenuHide();
    }
}
