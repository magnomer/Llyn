using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Llyn.Tests;

internal static class TInterfaceDeportment
{
    internal static QLecternReflex TLecternCreate(CDisplay display, FrameworkElement surface) =>
        new(surface, display.CDisplaySound);

    internal static (bool CDeskBackward, bool CDeskForward) TDeskChronicleRead(this CDesk desk) =>
        desk.CDeskChronicle.CDeskChronicleRead();

    internal static QPosture TPostureCreate(string root)
    {
        QPosture posture = new();
        posture.QPostureRootRefine(root);
        return posture;
    }

    internal static IReadOnlyList<QIndexItem> TIndexItemBuild(IReadOnlyList<CVistaRow> rows) =>
        QIndexItem.QIndexItemBuild(rows);

    internal static bool TIndexItemMatch(QIndexItem held, QIndexItem fresh) => QIndexItem.QIndexItemMatch(held, fresh);

    internal static void TIndexItemSync(QIndexItem held, QIndexItem fresh) => QIndexItem.QIndexItemSync(held, fresh);

    internal static bool TCaretEdgeApply(string key, int caret, int length, int selection, Action<int> remove) =>
        QCaret.QCaretEdgeApply(key, caret, length, selection, remove);

    internal static bool TCaretStepApply(string key, int length, int selection, Func<int, bool> move, Action place) =>
        QCaret.QCaretStepApply(key, length, selection, move, place);

    internal static void TDisplayEntryShow(this CDisplay display, long id, LEntryDraft draft) =>
        display.LDisplayRule.LDisplaySound.LDisplaySoundShow(id, draft);

    internal static void TReflexCreate(FrameworkElement surface, CEditor editor) =>
        new QReflex(surface, new QAnchor(surface)).QReflexIntroduce(
            editor.CEditorDesk, editor.CEditorEntry, editor.CEditorKindred, editor.CEditorSounding);

    internal static void TCadenceCreate(FrameworkElement surface, CEditor editor, CLedger ledger, CEnvoy envoy) =>
        new QCadence(surface).QCadenceIntroduce(
            editor.CEditorDesk,
            editor.CEditorEntry,
            editor.CEditorTimbre,
            editor.CEditorSounding,
            editor.CEditorFold,
            ledger,
            envoy);

    internal static QLecternSound TLecternSoundCreate(
        CDisplay display, FrameworkElement surface, CLedger ledger, CEnvoy envoy) =>
        new(surface, display.CDisplaySound, display.CDisplayFold, ledger, envoy);

    internal static ToggleButton TFanqieSwitchRead(this QFanqie box) => box.QFanqieSwitch;

    internal static ToggleButton TScriptSwitchRead(this QScript box) => box.QScriptSwitch;

    internal static void TEtymologySourceShow(this QEtymology etymology, bool linked) =>
        etymology.QEtymologySourceShow([], linked);

    internal static (Visibility, Visibility) TEtymologyFaceRead(this QEtymology etymology)
    {
        UIElementCollection faces = ((Panel)((Panel)((Border)etymology.Child).Child).Children[1]).Children;
        return (faces[0].Visibility, faces[1].Visibility);
    }

    internal static void TReflexTypeRefine(QReflexItem row, CReflexField field, string text) =>
        row.QReflexTypeRefine(row.QReflexItemReflex.CReflexTypedApply(field, text));

    internal static object TCardCreate()
    {
        CStateWording blank = new(string.Empty, null, false, null);
        return new PCard(
            string.Empty,
            [],
            [],
            [],
            [],
            static card => card.PCardDefinition,
            new CCardDraft(0, 0, blank, blank, blank, [], [], [], [], [], [], [], false, false));
    }

    internal static void TCardLabelShow(object card, IReadOnlyList<CTagDraft> drafts) =>
        QLabel.QLabelShow((PCard)card, drafts);

    internal static bool TCardLabelMove(object card, int step) => ((PCard)card).PCardLabel.PCaretMove(step);

    internal static long? TCardLabelFind(object card, int step) =>
        ((PCard)card).PCardLabel.PCaretFind(step)?.PLabelChipId;

    internal static (List<long>, int) TCardLabelRead(object card)
    {
        PCard held = (PCard)card;
        List<long> ids = [];
        foreach (object row in held.PCardLabel.PCaretRow)
        {
            ids.Add(((PLabelChip)row).PLabelChipId);
        }

        return (ids, held.PCardLabel.PCaretPosition);
    }

    internal static TextBox TCardPositionAttach(object card)
    {
        FrameworkElementFactory face = new(typeof(TextBox)) { Name = "PCardPositionText" };
        ItemsControl list = new()
        {
            Template = new ControlTemplate(typeof(ItemsControl))
            {
                VisualTree = new FrameworkElementFactory(typeof(ItemsPresenter)),
            },
            ItemTemplate = new DataTemplate { VisualTree = face },
            ItemsSource = new[] { card },
        };
        list.Measure(new Size(400, 400));
        list.Arrange(new Rect(0, 0, 400, 400));
        list.UpdateLayout();
        QLookItem.QLookItemAttach(
            list, (container, item, changed) => QCard.QCardRowRefine(container, (PCard)item, changed));
        FrameworkElement row = (FrameworkElement)list.ItemContainerGenerator.ContainerFromIndex(0);
        return QLook.QLookPartFind<TextBox>(row, "PCardPositionText")!;
    }

    internal static void TCardPositionShow(object card) => ((PCard)card).PCardPositionActive = true;

    internal static void TCardPositionHide(object card) => ((PCard)card).PCardPositionHide();

    internal static void TCardPositionSet(object card, int position) => ((PCard)card).PCardPosition = position;

    internal static object TCardFoldCreate(CCardDraft draft)
    {
        PCard card = new(string.Empty, [], [], [], [], static row => row.PCardDefinition, draft);
        card.PCardFoldShow(draft.CCardDraftFolded);
        return card;
    }

    internal static void TCardRowRefine(FrameworkElement container, object card) =>
        QCard.QCardRowRefine(container, (PCard)card, null);

    internal static void TCardHingeAttach(ItemsControl list, CEditor editor)
    {
        QCard card = new(null!, null!, null!, null!, null!, null!, null!, null!, null!, null!, [], [], []);
        card.QCardIntroduce(editor.CEditorField, editor.CEditorList);
        QLookItem.QLookItemAttach(list, card.QCardApply);
    }

    internal static object? TLookSettingRead(string style, string part, DependencyProperty property, bool folded)
    {
        QLookCue active = folded ? QLookCue.QLookCueChecked : QLookCue.QLookCueBase;
        QLook.QLookSetter? winner = QLookSheet.QLookSheetState.LastOrDefault(row =>
            row.QLookSetterStyle == style
            && row.QLookSetterPart == part
            && row.QLookSetterProperty == property
            && (active & row.QLookSetterCue) == row.QLookSetterCue);
        return (winner?.QLookSetterValue as QLookValue.QLookFix<Freezable>)?.QLookFixSetting;
    }

    internal static object TLeafItemCreate(CLeaf leaf, CStateWording peek) => new QLeafItem(leaf, peek);

    internal static void TLeafCardRefine(FrameworkElement container, object card) =>
        PLeaf.PLeafCardRefine(container, card, null);

    internal static QLecternCard TLecternCardCreate(CDisplay display, FrameworkElement surface) =>
        new(surface, display.CDisplayCard, display.CDisplayRoute, display.CDisplaySound);

    internal static void TCardRowShow(
        ObservableCollection<QTranscriptionItem> rows, IReadOnlyList<CTranscriptionDraft> drafts) =>
        QLookItem.QLookItemShow(
            rows,
            drafts,
            static row => row.QTranscriptionItemId,
            static draft => draft.CTranscriptionDraftId,
            QTranscriptionItem.QTranscriptionRowRefine,
            QTranscriptionItem.QTranscriptionStateRefine);

    internal static void TTranscriptionSchemeRefine(this QTranscriptionItem row, IReadOnlyList<CScheme> schemes) =>
        row.QTranscriptionSchemeRefine(schemes);

    internal static object TLocalizationCreate(FrameworkElement settings, CLedger ledger, CEnvoy envoy)
    {
        QLocalization localization = new(settings);
        localization.QLocalizationIntroduce(ledger, envoy);
        return localization;
    }

    internal static void TLocalizationRefine(
        object localization, IReadOnlyList<KeyValuePair<string, string>> languages, string chosen) =>
        ((QLocalization)localization).QLocalizationRefine(languages, chosen);

    internal static object TInflectionCreate(FrameworkElement settings, CLedger ledger, CEnvoy envoy)
    {
        QInflection analysis = new(settings);
        analysis.QInflectionIntroduce(ledger, envoy);
        return analysis;
    }

    internal static void TInflectionRefine(object analysis) => ((QInflection)analysis).QInflectionRefine();
}
