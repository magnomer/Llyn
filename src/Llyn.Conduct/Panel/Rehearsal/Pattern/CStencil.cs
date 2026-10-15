using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CStencil
{
    private const int CStencilSides = 5;

    private const int CStencilCategories = 4;

    internal static readonly IReadOnlyList<CStencilSlot> CStencilOffer =
    [
        .. Enumerable.Range(1, CStencilSides).SelectMany(static side => new[]
        {
            LStencilOfferCreate(LCueColumn.LCueColumnText, side, "Stencil.Side", true),
            LStencilOfferCreate(LCueColumn.LCueColumnPicture, side, "Stencil.Picture", true),
            LStencilOfferCreate(LCueColumn.LCueColumnSound, side, "Stencil.Sound", true),
        }),
        LStencilOfferCreate(LCueColumn.LCueColumnNotes, 1, "Stencil.Notes", false),
        LStencilOfferCreate(LCueColumn.LCueColumnExtra, 1, "Stencil.ExtraInfo", false),
        .. Enumerable.Range(1, CStencilCategories).Select(static index =>
            LStencilOfferCreate(LCueColumn.LCueColumnCategory, index, "Stencil.Category", true)),
    ];

    private static readonly LStencilField[] CStencilFields =
    [
        .. Enum.GetValues<LStencilField>().Where(static field =>
            field is not (LStencilField.LStencilFieldImage or LStencilField.LStencilFieldRecording)),
    ];

    private static readonly CStencilJoin[] CStencilJoins =
        [new CStencilJoin(false, "Stencil.JoinFirst"), new CStencilJoin(true, "Stencil.JoinAll")];

    private readonly LDeckPort _cStencilPort;

    private readonly CEnvoy _cStencilEnvoy;

    private readonly LSettingsPort _cStencilSettings;

    private readonly Func<long?> _cStencilChosen;

    private bool _cStencilText;

    internal CStencil(CAtelier atelier, CEnvoy envoy, Func<long?> chosen)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(chosen);

        _cStencilPort = atelier.CAtelierEntryBundle.CEntryBundleDeck;
        _cStencilEnvoy = envoy;
        _cStencilSettings = atelier.CAtelierSettingsPort;
        _cStencilChosen = chosen;
    }

    public event Action? CStencilChanged;

    public bool CStencilTextChecked => _cStencilText;

    public CStencilState? CStencilRead()
    {
        if (_cStencilChosen() is not long deck)
        {
            return null;
        }

        LStencil owned;
        IReadOnlyList<LStencil> saved;
        string grain;
        try
        {
            owned = _cStencilPort.LEngineStencilRead(deck);
            saved = _cStencilPort.LEngineStencilRead();
            grain = _cStencilPort.LEngineDeckRead(deck) is { } stored
                ? CDeck.LGrainKeyRead(stored.LDeckStored.LDeckGrain)
                : string.Empty;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cStencilEnvoy, _cStencilSettings, "Deck.LoadFailed", exception);
            return new CStencilState([], [], [], [], [], 0, 0, string.Empty);
        }

        return new CStencilState(
            CStencilOffer
                .Select(offer => offer with
                {
                    CStencilSlotBody = owned.LStencilSlots
                        .FirstOrDefault(slot => (int)slot.LStencilSlotColumn == offer.CStencilSlotColumn
                            && slot.LStencilSlotIndex == offer.CStencilSlotIndex)?
                        .LStencilSlotBody ?? string.Empty,
                })
                .ToList(),
            saved.Select(static stencil => new CStencilRow(stencil.LStencilId, stencil.LStencilName)).ToList(),
            Enumerable.Range(1, CStencilSides).Select(side => LStencilSideCreate(owned, side)).ToList(),
            CStencilFields.Select(static field => new CStencilField(LStencilKeyRead(field))).ToList(),
            CStencilJoins,
            owned.LStencilSlots.FirstOrDefault(static slot => slot.LStencilSlotColumn == LCueColumn.LCueColumnPicture)?
                .LStencilSlotIndex ?? 0,
            owned.LStencilSlots.FirstOrDefault(static slot => slot.LStencilSlotColumn == LCueColumn.LCueColumnSound)?
                .LStencilSlotIndex ?? 0,
            grain);
    }

    public void CStencilSlotSet(int column, int index, string body)
    {
        ArgumentNullException.ThrowIfNull(body);

        if (_cStencilChosen() is long deck)
        {
            LStencilSave(() => _cStencilPort.LEngineSlotSet(deck, new LStencilSlot((LCueColumn)column, index, body)));
        }
    }

    public void CStencilSave()
    {
        if (_cStencilChosen() is long deck && _cStencilEnvoy.CEnvoyCoinageRead("Coinage.Stencil") is string name)
        {
            LStencilSave(() => _cStencilPort.LEngineStencilSave(deck, name));
        }
    }

    public void CStencilApply(long stencil)
    {
        if (_cStencilChosen() is long deck)
        {
            LStencilSave(() => _cStencilPort.LEngineStencilApply(deck, stencil));
        }
    }

    public void CStencilFieldAppend(int side, int field)
    {
        if (field >= 0 && field < CStencilFields.Length && _cStencilChosen() is long deck)
        {
            LStencilSave(() => _cStencilPort.LEngineFieldAppend(deck, side, CStencilFields[field]));
        }
    }

    public void CStencilFieldRemove(int side, int place)
    {
        if (_cStencilChosen() is long deck)
        {
            LStencilSave(() => _cStencilPort.LEngineFieldRemove(deck, side, place));
        }
    }

    public void CStencilJoinSet(int side, int place, int join)
    {
        if (join >= 0 && join < CStencilJoins.Length && _cStencilChosen() is long deck)
        {
            LStencilSave(() => _cStencilPort.LEngineJoinSet(deck, side, place, CStencilJoins[join].CStencilJoinValue));
        }
    }

    public void CStencilMediaSet(int column, int side)
    {
        if (_cStencilChosen() is long deck)
        {
            LStencilSave(() => _cStencilPort.LEngineMediaSet(deck, (LCueColumn)column, side));
        }
    }

    public void CStencilPictureSet(int side)
    {
        CStencilMediaSet((int)LCueColumn.LCueColumnPicture, side);
    }

    public void CStencilSoundSet(int side)
    {
        CStencilMediaSet((int)LCueColumn.LCueColumnSound, side);
    }

    public void CStencilTextToggle()
    {
        _cStencilText = !_cStencilText;
        CStencilChanged?.Invoke();
    }

    private static CStencilSide LStencilSideCreate(LStencil owned, int side)
    {
        string body = owned.LStencilSlots
            .FirstOrDefault(slot =>
                slot.LStencilSlotColumn == LCueColumn.LCueColumnText && slot.LStencilSlotIndex == side)?
            .LStencilSlotBody ?? string.Empty;
        bool custom = !LDeckPort.LEngineBodyCheck(body);
        IReadOnlyList<CStencilChip> chips = custom
            ? []
            : LDeckPort.LEngineTokenParse(body)
                .Where(static token => token.LStencilTokenField is not null)
                .Select(static token => new CStencilChip(
                    LStencilKeyRead(token.LStencilTokenField!.Value),
                    token.LStencilTokenAll))
                .ToList();
        return new CStencilSide(side, chips, custom);
    }

    private static string LStencilKeyRead(LStencilField field)
    {
        return field switch
        {
            LStencilField.LStencilFieldHeadword => "Stencil.SlotHeadword",
            LStencilField.LStencilFieldForms => "Stencil.SlotForms",
            LStencilField.LStencilFieldPronunciation => "Stencil.SlotPronunciation",
            LStencilField.LStencilFieldRespelling => "Stencil.SlotRespelling",
            LStencilField.LStencilFieldRecording => "Stencil.SlotRecording",
            LStencilField.LStencilFieldSpeech => "Stencil.SlotSpeech",
            LStencilField.LStencilFieldUnit => "Stencil.SlotUnit",
            LStencilField.LStencilFieldFreedom => "Stencil.SlotFreedom",
            LStencilField.LStencilFieldDefinition => "Stencil.SlotDefinition",
            LStencilField.LStencilFieldExpression => "Stencil.SlotExpression",
            LStencilField.LStencilFieldParaphrase => "Stencil.SlotParaphrase",
            LStencilField.LStencilFieldSentence => "Stencil.SlotSentence",
            LStencilField.LStencilFieldGloss => "Stencil.SlotGloss",
            LStencilField.LStencilFieldTarget => "Stencil.SlotTarget",
            LStencilField.LStencilFieldImage => "Stencil.SlotImage",
            LStencilField.LStencilFieldTag => "Stencil.SlotTag",
            LStencilField.LStencilFieldRegister => "Stencil.SlotRegister",
            LStencilField.LStencilFieldSituation => "Stencil.SlotSituation",
            LStencilField.LStencilFieldSource => "Stencil.SlotSource",
            _ => throw new ArgumentOutOfRangeException(nameof(field), field, null),
        };
    }

    private static CStencilSlot LStencilOfferCreate(LCueColumn column, int index, string caption, bool numbered)
    {
        return new CStencilSlot(
            (int)column,
            index,
            string.Empty,
            caption,
            numbered ? index.ToString(CultureInfo.InvariantCulture) : string.Empty);
    }

    private void LStencilSave(Action save)
    {
        try
        {
            save();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cStencilEnvoy, _cStencilSettings, "Stencil.SaveFailed", exception);
            return;
        }

        CStencilChanged?.Invoke();
    }
}
