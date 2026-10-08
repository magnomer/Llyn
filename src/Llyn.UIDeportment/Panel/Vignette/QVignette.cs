using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QVignette
{
    private readonly UserControl _qVignetteScope;

    private CRepertoire _cRepertoire = null!;

    private CAtelier _cAtelier = null!;

    internal QVignette(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qVignetteScope = scope;
    }

    private Grid QVignetteView => QContract.QContractFind<Grid>(_qVignetteScope, "PVignette");

    private StackPanel QVignetteBody => QContract.QContractFind<StackPanel>(_qVignetteScope, "PVignetteBody");

    private TextBlock QVignetteTitle => QContract.QContractFind<TextBlock>(_qVignetteScope, "PVignetteTitle");

    private Border QVignetteChip => QContract.QContractFind<Border>(_qVignetteScope, "PVignetteChip");

    private TextBlock QVignetteKind => QContract.QContractFind<TextBlock>(_qVignetteScope, "PVignetteKind");

    private TextBlock QVignetteTally => QContract.QContractFind<TextBlock>(_qVignetteScope, "PVignetteTally");

    private StackPanel QVignetteDescriptionSection =>
        QContract.QContractFind<StackPanel>(_qVignetteScope, "PVignetteDescriptionSection");

    private StackPanel QVignetteDescription =>
        QContract.QContractFind<StackPanel>(_qVignetteScope, "PVignetteDescription");

    private ItemsControl QVignettePicture =>
        QContract.QContractFind<ItemsControl>(_qVignetteScope, "PVignettePicture");

    private ItemsControl QVignetteVideo => QContract.QContractFind<ItemsControl>(_qVignetteScope, "PVignetteVideo");

    private TextBlock QVignetteUnselected =>
        QContract.QContractFind<TextBlock>(_qVignetteScope, "PVignetteUnselected");

    internal void QVignetteIntroduce(CRepertoire repertoire, CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(repertoire);
        ArgumentNullException.ThrowIfNull(atelier);

        _cRepertoire = repertoire;
        _cAtelier = atelier;
        CPanel atlas = _cRepertoire.CRepertoireAtlas.CAtlasPanel;
        _cRepertoire.CRepertoireSituationChanged += QVignetteRefine;
        atlas.CPanelAperture.CApertureRowsChanged += QVignetteTallyRefine;
        atlas.CPanelCleared += QVignetteClearRefine;

        QLookItem.QLookItemAttach(QVignettePicture, QImageItem.QImageItemApply);
        QLookItem.QLookItemAttach(QVignetteVideo, QVideoItem.QVideoItemApply);
    }

    internal void QVignetteVisibleRefine()
    {
        QVignetteView.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireDiptych.CDiptychParentShown);
        QVignetteBody.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireVignetteHeld);
        QVignetteUnselected.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireVignetteBlank);
    }

    private void QVignetteTallyRefine()
    {
        QVignetteTally.Text = _cRepertoire.CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureTallyRead();
    }

    private void QVignetteRefine(CSituation situation)
    {
        QVignetteTitleRefine(situation.CSituationTitle);
        QVignetteKindRefine(situation.CSituationKind);
        QVignetteDescriptionRefine(situation);
        QVignetteMediaRefine(
            [.. situation.CSituationImage.Select(static draft => new QImageItem(draft))],
            [.. situation.CSituationVideo.Select(static draft => new QVideoItem(draft))]);
        QVignetteTallyRefine();
    }

    private void QVignetteClearRefine()
    {
        QVignetteMediaRefine(null, null);
    }

    private void QVignetteTitleRefine(CStateWording title)
    {
        QVignetteTitle.Text = title.CStateWordingKey is string key
            ? QLocalizationCatalog.QLocalizationTextRead(key)
            : title.CStateWordingText;
        QVignetteTitle.SetResourceReference(
            TextBlock.ForegroundProperty,
            title.CStateWordingMuted ? "Theme.Muted" : "Theme.Ink");
    }

    private void QVignetteKindRefine(CStateWording kind)
    {
        QVignetteKind.Text = kind.CStateWordingKey is string key
            ? QLocalizationCatalog.QLocalizationTextRead(key)
            : kind.CStateWordingText;
        QVignetteChip.Visibility = QLook.QLookVisibleRead(!kind.CStateWordingMuted);
    }

    private void QVignetteDescriptionRefine(CSituation situation)
    {
        if (situation.CSituationDescription.CStateWordingKey is string key)
        {
            QMarkdownFace.QMarkdownRefine(QVignetteDescription, QLocalizationCatalog.QLocalizationTextRead(key));
        }
        else
        {
            QMarkdownFace.QMarkdownRefine(QVignetteDescription, situation.CSituationMarkdown, _cAtelier);
        }

        QVignetteDescriptionSection.Visibility =
            QLook.QLookVisibleRead(!situation.CSituationDescription.CStateWordingMuted);
    }

    private void QVignetteMediaRefine(IReadOnlyList<QImageItem>? pictures, IReadOnlyList<QVideoItem>? videos)
    {
        QVignettePicture.ItemsSource = pictures;
        QVignetteVideo.ItemsSource = videos;
    }
}
