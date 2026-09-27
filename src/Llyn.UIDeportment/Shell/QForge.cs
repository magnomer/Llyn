using System;

namespace Llyn.UIDeportment;

public sealed class QForge
{
    private readonly LWindow _qForgeWindow;

    internal QForge(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        _qForgeWindow = window;
    }

    public LEditor QForgeEditorCreate(Func<bool> unreadableSeam)
    {
        return new LEditor(
            _qForgeWindow.LWindowAtelier.CAtelierDraftPort,
            _qForgeWindow.LWindowAtelier.CAtelierEntryPort,
            _qForgeWindow.LWindowAtelier.CAtelierPhonologyPort,
            _qForgeWindow.LWindowAtelier.CAtelierSettingsPort,
            _qForgeWindow.LWindowAtelier.CAtelierMediaPort,
            unreadableSeam);
    }

    public LEditor QForgeInputCreate(Func<bool> unreadableSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            QForgeEditorCreate(unreadableSeam), static (editor, window) => editor.LEditorVistaRestore(window));
    }

    public LCorpus QForgeCorpusCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<Func<bool, bool>, bool> leaveSeam,
        Func<int, bool> removalSeam, Func<bool> unreadableSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            new LCorpus(
                _qForgeWindow.LWindowAtelier.CAtelierDraftPort,
                _qForgeWindow.LWindowAtelier.CAtelierEntryPort,
                _qForgeWindow.LWindowAtelier.CAtelierPortraitPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                removalSeam,
                unreadableSeam),
            static (corpus, window) => corpus.LCorpusVistaRestore(window));
    }

    public LFavorite QForgeFavoriteCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            new LFavorite(
                _qForgeWindow.LWindowAtelier.CAtelierEntryPort,
                _qForgeWindow.LWindowAtelier.CAtelierPortraitPort,
                _qForgeWindow.LWindowAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            static (favorite, window) => favorite.LFavoriteVistaRestore(window));
    }

    public LGuild QForgeGuildCreate(
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam,
        Func<string, string, bool> unionSeam,
        Func<bool> unreadableSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            new LGuild(
                _qForgeWindow.LWindowAtelier.CAtelierDraftPort,
                _qForgeWindow.LWindowAtelier.CAtelierEntryPort,
                _qForgeWindow.LWindowAtelier.CAtelierPortraitPort,
                _qForgeWindow.LWindowAtelier.CAtelierSettingsPort,
                shownSeam,
                leaveSeam,
                removalSeam,
                unionSeam,
                unreadableSeam),
            static (guild, window) => guild.LGuildVistaRestore(window));
    }

    public LLibrary QForgeLibraryCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            new LLibrary(
                _qForgeWindow.LWindowAtelier.CAtelierEntryPort,
                _qForgeWindow.LWindowAtelier.CAtelierPortraitPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            static (library, window) => library.LLibraryVistaRestore(window));
    }

    public LPhonology QForgePhonologyCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            new LPhonology(
                _qForgeWindow.LWindowAtelier.CAtelierPhonologyPort,
                _qForgeWindow.LWindowAtelier.CAtelierPortraitPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            static (phonology, window) => phonology.LPhonologyVistaRestore(window));
    }

    public LShelf QForgeShelfCreate(
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam,
        Func<bool> unreadableSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            new LShelf(
                _qForgeWindow.LWindowAtelier.CAtelierDraftPort,
                _qForgeWindow.LWindowAtelier.CAtelierEntryPort,
                _qForgeWindow.LWindowAtelier.CAtelierPortraitPort,
                _qForgeWindow.LWindowAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                removalSeam,
                unreadableSeam),
            static (shelf, window) => shelf.LShelfVistaRestore(window));
    }

    public LRepertoire QForgeRepertoireCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<Func<bool, bool>, bool> leaveSeam,
        Func<int, bool> removalSeam, Func<bool> unreadableSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            new LRepertoire(
                _qForgeWindow.LWindowAtelier.CAtelierDraftPort,
                _qForgeWindow.LWindowAtelier.CAtelierEntryPort,
                _qForgeWindow.LWindowAtelier.CAtelierPortraitPort,
                _qForgeWindow.LWindowAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                removalSeam,
                unreadableSeam),
            static (repertoire, window) => repertoire.LRepertoireVistaRestore(window));
    }

    public LTaxonomy QForgeTaxonomyCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            new LTaxonomy(
                _qForgeWindow.LWindowAtelier.CAtelierEntryPort,
                _qForgeWindow.LWindowAtelier.CAtelierPortraitPort,
                _qForgeWindow.LWindowAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            static (taxonomy, window) => taxonomy.LTaxonomyVistaRestore(window));
    }

    public LTenor QForgeTenorCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            new LTenor(
                _qForgeWindow.LWindowAtelier.CAtelierEntryPort,
                _qForgeWindow.LWindowAtelier.CAtelierPortraitPort,
                _qForgeWindow.LWindowAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            static (tenor, window) => tenor.LTenorVistaRestore(window));
    }

    public LXiesheng QForgeXieshengCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            new LXiesheng(
                _qForgeWindow.LWindowAtelier.CAtelierPhonologyPort,
                _qForgeWindow.LWindowAtelier.CAtelierPortraitPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            static (xiesheng, window) => xiesheng.LXieshengVistaRestore(window));
    }

    public LYunjing QForgeYunjingCreate(
        LEditor editor, LLectern lectern, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return _qForgeWindow.LWindowVistaAdd(
            new LYunjing(
                _qForgeWindow.LWindowAtelier.CAtelierPhonologyPort,
                _qForgeWindow.LWindowAtelier.CAtelierPortraitPort,
                _qForgeWindow.LWindowAtelier.CAtelierSettingsPort,
                editor,
                lectern,
                shownSeam,
                leaveSeam,
                deleteSeam),
            static (yunjing, window) => yunjing.LYunjingVistaRestore(window));
    }
}
