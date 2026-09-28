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
        return new LEditor(CEditor.CEditorCreate(_qForgeAtelier, envoy));
    }

    public LEditor QForgeInputCreate(CEnvoy envoy)
    {
        return QForgeVistaAdd(
            QForgeEditorCreate(envoy), editor => _qForgeAtelier.CAtelierInputRestore(editor.LEditorStudio));
    }

    public CCorpus QForgeCorpusCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            CCorpus.CCorpusCreate(_qForgeAtelier, shownSeam, envoy),
            static corpus => corpus.CCorpusVistaRestore());
    }

    public CFavorite QForgeFavoriteCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            CFavorite.CFavoriteCreate(_qForgeAtelier, shownSeam, envoy),
            static favorite => favorite.CFavoriteVistaRestore());
    }

    public CGuild QForgeGuildCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            CGuild.CGuildCreate(_qForgeAtelier, shownSeam, envoy),
            static guild => guild.CGuildVistaRestore());
    }

    public CLibrary QForgeLibraryCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            CLibrary.CLibraryCreate(_qForgeAtelier, shownSeam, envoy),
            static library => library.CLibraryVistaRestore());
    }

    public LPhonology QForgePhonologyCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            new LPhonology(
                _qForgeAtelier.CAtelierPhonologyPort,
                _qForgeAtelier.CAtelierPortraitPort,
                editor,
                lectern,
                shownSeam,
                envoy),
            phonology => phonology.LPhonologyVistaRestore(_qForgeAtelier));
    }

    public CShelf QForgeShelfCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            CShelf.CShelfCreate(_qForgeAtelier, shownSeam, envoy),
            static shelf => shelf.CShelfVistaRestore());
    }

    public CRepertoire QForgeRepertoireCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            CRepertoire.CRepertoireCreate(_qForgeAtelier, shownSeam, envoy),
            static repertoire => repertoire.CRepertoireVistaRestore());
    }

    public CTaxonomy QForgeTaxonomyCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            CTaxonomy.CTaxonomyCreate(_qForgeAtelier, shownSeam, envoy),
            static taxonomy => taxonomy.CTaxonomyVistaRestore());
    }

    public CTenor QForgeTenorCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            CTenor.CTenorCreate(_qForgeAtelier, shownSeam, envoy),
            static tenor => tenor.CTenorVistaRestore());
    }

    public LXiesheng QForgeXieshengCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            new LXiesheng(
                _qForgeAtelier.CAtelierPhonologyPort,
                _qForgeAtelier.CAtelierPortraitPort,
                editor,
                lectern,
                shownSeam,
                envoy),
            xiesheng => xiesheng.LXieshengVistaRestore(_qForgeAtelier));
    }

    public LYunjing QForgeYunjingCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, CEnvoy envoy)
    {
        return QForgeVistaAdd(
            new LYunjing(
                _qForgeAtelier.CAtelierPhonologyPort,
                _qForgeAtelier.CAtelierPortraitPort,
                _qForgeAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                envoy),
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
