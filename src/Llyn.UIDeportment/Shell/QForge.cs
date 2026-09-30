using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QForge
{
    private readonly CAtelier _qForgeAtelier;

    internal QForge(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _qForgeAtelier = atelier;
    }

    public CEditor QForgeInputCreate(CEnvoy envoy)
    {
        return _qForgeAtelier.CAtelierInputCreate(envoy);
    }

    public CCorpus QForgeCorpusCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        CCorpus corpus = CCorpus.CCorpusCreate(
            _qForgeAtelier, shownSeam, envoy, LObserver.LObserverCreate<Action>(static run => run()));
        corpus.CCorpusVistaRestore();
        return corpus;
    }

    public CFavorite QForgeFavoriteCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        CFavorite favorite = CFavorite.CFavoriteCreate(
            _qForgeAtelier, shownSeam, envoy, LObserver.LObserverCreate<Action>(static run => run()));
        favorite.CFavoriteVistaRestore();
        return favorite;
    }

    public CGuild QForgeGuildCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        CGuild guild = CGuild.CGuildCreate(
            _qForgeAtelier, shownSeam, envoy, LObserver.LObserverCreate<Action>(static run => run()));
        guild.CGuildVistaRestore();
        return guild;
    }

    public CLibrary QForgeLibraryCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        CLibrary library = CLibrary.CLibraryCreate(
            _qForgeAtelier, shownSeam, envoy, LObserver.LObserverCreate<Action>(static run => run()));
        library.CLibraryVistaRestore();
        return library;
    }

    public CPhonology QForgePhonologyCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        CPhonology phonology = CPhonology.CPhonologyCreate(
            _qForgeAtelier, shownSeam, envoy, LObserver.LObserverCreate<Action>(static run => run()));
        phonology.CPhonologyVistaRestore();
        return phonology;
    }

    public CShelf QForgeShelfCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        CShelf shelf = CShelf.CShelfCreate(
            _qForgeAtelier, shownSeam, envoy, LObserver.LObserverCreate<Action>(static run => run()));
        shelf.CShelfVistaRestore();
        return shelf;
    }

    public CRepertoire QForgeRepertoireCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        CRepertoire repertoire = CRepertoire.CRepertoireCreate(
            _qForgeAtelier, shownSeam, envoy, LObserver.LObserverCreate<Action>(static run => run()));
        repertoire.CRepertoireVistaRestore();
        return repertoire;
    }

    public CTaxonomy QForgeTaxonomyCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        CTaxonomy taxonomy = CTaxonomy.CTaxonomyCreate(
            _qForgeAtelier, shownSeam, envoy, LObserver.LObserverCreate<Action>(static run => run()));
        taxonomy.CTaxonomyVistaRestore();
        return taxonomy;
    }

    public CTenor QForgeTenorCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        CTenor tenor = CTenor.CTenorCreate(
            _qForgeAtelier, shownSeam, envoy, LObserver.LObserverCreate<Action>(static run => run()));
        tenor.CTenorVistaRestore();
        return tenor;
    }

    public CXiesheng QForgeXieshengCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        CXiesheng xiesheng = CXiesheng.CXieshengCreate(_qForgeAtelier, shownSeam, envoy);
        xiesheng.CXieshengVistaRestore();
        return xiesheng;
    }

    public CYunjing QForgeYunjingCreate(Func<bool> shownSeam, CEnvoy envoy)
    {
        CYunjing yunjing = CYunjing.CYunjingCreate(_qForgeAtelier, shownSeam, envoy);
        yunjing.CYunjingVistaRestore();
        return yunjing;
    }
}
