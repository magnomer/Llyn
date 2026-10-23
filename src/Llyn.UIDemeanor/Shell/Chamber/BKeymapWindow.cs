using System;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal static class BKeymapWindow
{
    internal static BKeymap BKeymapWindowBuild(
        Action<BKeymapFocus> help,
        Action quit,
        Action navigation,
        Action close,
        Action back,
        Action forward,
        Func<CNavigationState?> state)
    {
        ArgumentNullException.ThrowIfNull(help);
        ArgumentNullException.ThrowIfNull(quit);
        ArgumentNullException.ThrowIfNull(navigation);
        ArgumentNullException.ThrowIfNull(close);
        ArgumentNullException.ThrowIfNull(back);
        ArgumentNullException.ThrowIfNull(forward);
        ArgumentNullException.ThrowIfNull(state);

        Func<BKeymapLive> shown = static () => BKeymapLive.BKeymapLiveShown;
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ => action();
        Action<ConsoleKeyInfo> quitting = pressed(quit);
        Func<Action, Action<ConsoleKeyInfo>> closing = action => _ =>
        {
            close();
            action();
        };
        Func<Func<CVoyageState, bool>, Func<BKeymapLive>> voyage = can => () =>
            state() is not CNavigationState current ? BKeymapLive.BKeymapLiveHidden
            : can(current.CNavigationStateVoyage) ? BKeymapLive.BKeymapLiveShown
            : BKeymapLive.BKeymapLiveDim;
        Func<BKeymapLive> past = voyage(static current => current.CVoyageStatePast);
        Func<BKeymapLive> future = voyage(static current => current.CVoyageStateFuture);
        Func<BKeymapLive> open = voyage(static _ => true);
        const ConsoleModifiers alt = ConsoleModifiers.Alt;
        const BKeymapFocus window = BKeymapFocus.BKeymapFocusWindow;
        const BKeymapFocus parley = BKeymapFocus.BKeymapFocusParley;
        const BKeymapFocus notice = BKeymapFocus.BKeymapFocusNotice;
        const BKeymapFocus helping = BKeymapFocus.BKeymapFocusHelp;
        const BKeymapFocus roof = BKeymapFocus.BKeymapFocusRoof;
        return new BKeymap(
        [
            new(ConsoleKey.F1, 0, window, "Legend.Help", _ => help(window), shown),
            new(ConsoleKey.F10, 0, window, "Legend.Quit", quitting, shown),
            new(ConsoleKey.C, ConsoleModifiers.Control, window, "Legend.Quit", quitting, shown),
            new(ConsoleKey.F10, 0, notice, "Legend.Quit", quitting, shown),
            new(ConsoleKey.C, ConsoleModifiers.Control, notice, "Legend.Quit", quitting, shown),
            new(ConsoleKey.F1, 0, parley, "Legend.Help", _ => help(parley), shown),
            new(ConsoleKey.C, ConsoleModifiers.Control, parley, "Legend.Quit", quitting, shown),
            new(ConsoleKey.C, ConsoleModifiers.Control, helping, "Legend.Quit", quitting, shown),
            new(ConsoleKey.F1, 0, roof, "Legend.Help", closing(() => help(roof)), shown),
            new(ConsoleKey.F10, 0, roof, "Legend.Quit", closing(quit), shown),
            new(ConsoleKey.C, ConsoleModifiers.Control, roof, "Legend.Quit", closing(quit), shown),
            new(ConsoleKey.F1, alt, window, "Window.Help.Choose", pressed(navigation), open),
            new(ConsoleKey.LeftArrow, alt, window, "Voyage.Retreat", pressed(back), past),
            new(ConsoleKey.RightArrow, alt, window, "Voyage.Advance", pressed(forward), future),
            new(ConsoleKey.F1, alt, roof, "Window.Help.Choose", closing(navigation), open),
            new(ConsoleKey.LeftArrow, alt, roof, "Voyage.Retreat", closing(back), past),
            new(ConsoleKey.RightArrow, alt, roof, "Voyage.Advance", closing(forward), future),
        ]);
    }
}
