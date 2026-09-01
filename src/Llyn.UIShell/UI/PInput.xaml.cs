using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

/// <summary>
/// The input panel as a control: what it is made of, and when it starts and stops. Its behaviour is
/// split across the files beside this one — the card lists, the drag, the menus, the form as a value;
/// this file only assembles the parts, takes the engine the window opened, and shuts the panel down.
/// </summary>
public partial class PInput : UserControl
{
    // The window this panel sits in. A panel does not put up its own dialogs: a failure and a
    // discard question are the program speaking, so they are asked for through the window.
    private PWindow _pInputHost = null!;

    private LEngine _lEngine = null!;

    public PInput()
    {
        InitializeComponent();

        // Each card and menu row is a template in a dictionary of its own, so this panel's markup
        // stays its own layout. A template still raises this panel's events, which is why the
        // dictionaries are built here against this instance rather than merged from markup.
        Resources.MergedDictionaries.Add(new PSenseTemplate(this));
        Resources.MergedDictionaries.Add(new PCollocationTemplate(this));
        Resources.MergedDictionaries.Add(new PLangcodeTemplate(this));
        Resources.MergedDictionaries.Add(new PLookupTemplate(this));
        Resources.MergedDictionaries.Add(new PDownloaderTemplate(this));

        PSenseList.ItemsSource = _pSenseList;
        PCollocationList.ItemsSource = _pCollocationList;
        PLookupMenuList.ItemsSource = _pLookupCandidate;
        PDownloaderMenuList.ItemsSource = _pDownloaderRecording;
        PLangcodeListMenu.ItemsSource = _pLangcodeList;
        PHeadword.TextChanged += PHeadwordHandle;
    }

    /// <summary>
    /// Puts the panel to work on <paramref name="engine"/>, the workspace the window opened, and
    /// opens the form on the session that workspace remembers.
    /// </summary>
    internal void PInputAttach(PWindow host, LEngine engine)
    {
        _pInputHost = host;
        _lEngine = engine;

        // The session the last run left behind: the form opens on the entry the workspace row names,
        // or empty when it names nothing. This also seeds the card lists, so no cards are added here.
        // It runs before the language menu is built, so the language it restores is already the
        // chosen one when the menu decides whether a fallback is needed.
        PStateRestore();

        PLangcodeLoad();
    }

    /// <summary>
    /// Stops the panel: searches in flight are called off, playback is released, and the entry the
    /// form stands on is recorded as the session to reopen on.
    /// </summary>
    internal void PInputClose()
    {
        PLookupCancel();
        PDownloaderCancel();
        _pDownloaderPlayer.Close();
        // The session is recorded while the engine is still open: the save runs through it.
        PStateSave();
    }
}
