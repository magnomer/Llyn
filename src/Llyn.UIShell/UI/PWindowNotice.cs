using System;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// What the window says to the user in its own voice: the localized text lookup every panel reads
/// its wording through, and the two message boxes the shell puts up — a request that failed, and the
/// question asked before typed work is thrown away.
/// </summary>
public partial class PWindow
{
    internal string PLocalizationTextRead(string key)
    {
        return TryFindResource(key) as string ?? key;
    }

    // Presents a request that failed, under the localized headline the given key names.
    // A deliberate refusal carries a reason key, which resolves through the same catalog as the rest
    // of the interface; anything unexpected is a fault rather than a refusal, so it keeps its own
    // message, which stays diagnosable even though it is not translated.
    internal void PWindowFailureShow(string key, Exception exception)
    {
        string detail = exception is LRefusal refusal
            ? PLocalizationTextRead(refusal.LRefusalReason)
            : exception.Message;

        MessageBox.Show(
            this,
            $"{PLocalizationTextRead(key)}\n\n{detail}",
            PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    // Asks before typed text is thrown away, and answers whether it may be. Every panel that edits an
    // entry is asked whether it holds unsaved work, because the window closing and the workspace
    // changing take all of them with it.
    internal bool PWindowDiscardConfirm()
    {
        return PWindowDiscardConfirm(PInput.PInputChangeCheck() || PList.PListChangeCheck());
    }

    // The same question over one panel's own answer, for a panel that is leaving its editing state
    // while the rest of the window stays as it is. Nothing unsaved means nothing to ask about, so the
    // question is only ever put when there is something to lose — which is why it can sit in front of
    // every path that discards typed work.
    internal bool PWindowDiscardConfirm(bool unsaved)
    {
        if (!unsaved)
        {
            return true;
        }

        return MessageBox.Show(
            this,
            PLocalizationTextRead("Input.DiscardConfirm"),
            PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}
