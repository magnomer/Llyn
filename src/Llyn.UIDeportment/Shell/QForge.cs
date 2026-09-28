using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QForge
{
    private readonly CAtelier _qForgeAtelier;

    private readonly List<Action> _qForgeVistas = [];

    internal QForge(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _qForgeAtelier = atelier;
    }

    public LEditor QForgeEditorCreate(CEnvoy envoy)
    {
        return new LEditor(
            CEditor.CEditorCreate(_qForgeAtelier, envoy),
            _qForgeAtelier.CAtelierPhonologyPort,
            _qForgeAtelier.CAtelierDraftPort);
    }

    public LEditor QForgeInputCreate(CEnvoy envoy)
    {
        return QForgeVistaAdd(
            QForgeEditorCreate(envoy), editor => _qForgeAtelier.CAtelierInputRestore(editor.LEditorStudio));
    }

    public LCorpus QForgeCorpusCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<Func<bool, bool>, bool> leaveSeam,
        Func<int, bool> removalSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            new LCorpus(
                _qForgeAtelier.CAtelierDraftPort,
                _qForgeAtelier.CAtelierEntryPort,
                _qForgeAtelier.CAtelierPortraitPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                removalSeam,
                envoy),
            corpus => corpus.LCorpusVistaRestore(_qForgeAtelier));
    }

    public LFavorite QForgeFavoriteCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return QForgeVistaAdd(
            new LFavorite(
                _qForgeAtelier.CAtelierEntryPort,
                _qForgeAtelier.CAtelierPortraitPort,
                _qForgeAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            favorite => favorite.LFavoriteVistaRestore(_qForgeAtelier));
    }

    public LGuild QForgeGuildCreate(
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam,
        Func<string, string, bool> unionSeam,
        CEnvoy envoy)
    {
        return QForgeVistaAdd(
            new LGuild(
                _qForgeAtelier.CAtelierDraftPort,
                _qForgeAtelier.CAtelierEntryPort,
                _qForgeAtelier.CAtelierPortraitPort,
                _qForgeAtelier.CAtelierSettingsPort,
                shownSeam,
                leaveSeam,
                removalSeam,
                unionSeam,
                envoy),
            guild => guild.LGuildVistaRestore(_qForgeAtelier));
    }

    public LLibrary QForgeLibraryCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return QForgeVistaAdd(
            new LLibrary(
                _qForgeAtelier.CAtelierEntryPort,
                _qForgeAtelier.CAtelierPortraitPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            library => library.LLibraryVistaRestore(_qForgeAtelier));
    }

    public LPhonology QForgePhonologyCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return QForgeVistaAdd(
            new LPhonology(
                _qForgeAtelier.CAtelierPhonologyPort,
                _qForgeAtelier.CAtelierPortraitPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            phonology => phonology.LPhonologyVistaRestore(_qForgeAtelier));
    }

    public LShelf QForgeShelfCreate(
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam,
        CEnvoy envoy)
    {
        return QForgeVistaAdd(
            new LShelf(
                _qForgeAtelier.CAtelierDraftPort,
                _qForgeAtelier.CAtelierEntryPort,
                _qForgeAtelier.CAtelierPortraitPort,
                _qForgeAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                removalSeam,
                envoy),
            shelf => shelf.LShelfVistaRestore(_qForgeAtelier));
    }

    public LRepertoire QForgeRepertoireCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<Func<bool, bool>, bool> leaveSeam,
        Func<int, bool> removalSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            new LRepertoire(
                _qForgeAtelier.CAtelierDraftPort,
                _qForgeAtelier.CAtelierEntryPort,
                _qForgeAtelier.CAtelierPortraitPort,
                _qForgeAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                removalSeam,
                envoy),
            repertoire => repertoire.LRepertoireVistaRestore(_qForgeAtelier));
    }

    public LTaxonomy QForgeTaxonomyCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return QForgeVistaAdd(
            new LTaxonomy(
                _qForgeAtelier.CAtelierEntryPort,
                _qForgeAtelier.CAtelierPortraitPort,
                _qForgeAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            taxonomy => taxonomy.LTaxonomyVistaRestore(_qForgeAtelier));
    }

    public LTenor QForgeTenorCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return QForgeVistaAdd(
            new LTenor(
                _qForgeAtelier.CAtelierEntryPort,
                _qForgeAtelier.CAtelierPortraitPort,
                _qForgeAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            tenor => tenor.LTenorVistaRestore(_qForgeAtelier));
    }

    public LXiesheng QForgeXieshengCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return QForgeVistaAdd(
            new LXiesheng(
                _qForgeAtelier.CAtelierPhonologyPort,
                _qForgeAtelier.CAtelierPortraitPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            xiesheng => xiesheng.LXieshengVistaRestore(_qForgeAtelier));
    }

    public LYunjing QForgeYunjingCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return QForgeVistaAdd(
            new LYunjing(
                _qForgeAtelier.CAtelierPhonologyPort,
                _qForgeAtelier.CAtelierPortraitPort,
                _qForgeAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            yunjing => yunjing.LYunjingVistaRestore(_qForgeAtelier));
    }

    public void QForgeVistaRestore()
    {
        foreach (Action restore in _qForgeVistas)
        {
            restore();
        }
    }

    private QForgeHeld QForgeVistaAdd<QForgeHeld>(QForgeHeld held, Action<QForgeHeld> restore)
    {
        restore(held);
        _qForgeVistas.Add(() => restore(held));
        return held;
    }
}
