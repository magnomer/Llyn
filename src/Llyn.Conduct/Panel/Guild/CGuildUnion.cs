using System;
using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CGuildUnion
{
    private readonly LAuthorPort _cGuildUnionPort;

    private readonly CEnvoy _cGuildUnionEnvoy;

    private readonly LSettingsPort _cGuildUnionSettings;

    private readonly CDesk _cGuildUnionAutograph;

    private readonly COeuvre _cGuildUnionOeuvre;

    private readonly CPanel _cGuildUnionPanel;

    private readonly Action<long> _cGuildUnionSeam;

    internal CGuildUnion(
        LAuthorPort port,
        CEnvoy envoy,
        LSettingsPort settings,
        CDesk autograph,
        COeuvre oeuvre,
        CPanel panel,
        Action<long> seam)
    {
        ArgumentNullException.ThrowIfNull(port);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(autograph);
        ArgumentNullException.ThrowIfNull(oeuvre);
        ArgumentNullException.ThrowIfNull(panel);
        ArgumentNullException.ThrowIfNull(seam);

        _cGuildUnionPort = port;
        _cGuildUnionEnvoy = envoy;
        _cGuildUnionSettings = settings;
        _cGuildUnionAutograph = autograph;
        _cGuildUnionOeuvre = oeuvre;
        _cGuildUnionPanel = panel;
        _cGuildUnionSeam = seam;
        autograph.CDeskStarted += () => CGuildUnionCleared?.Invoke();
    }

    public event Action? CGuildUnionCleared;

    public bool CGuildUnionShown => _cGuildUnionAutograph.CDeskStored;

    public IReadOnlyList<CCatalogAuthor> CGuildUnionRead(string typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        return _cGuildUnionOeuvre.LOeuvreAuthorRead(
            _cGuildUnionPort.LEngineUnionFind(_cGuildUnionPanel.CPanelAperture.CApertureVista, typed));
    }

    public void CGuildUnionSelect(long? id)
    {
        if (id is not long kept)
        {
            return;
        }

        if (_cGuildUnionPanel.CPanelAperture.CApertureVista?.LVistaStored is not long author)
        {
            return;
        }

        if (!LGuildUnionConfirm(kept))
        {
            return;
        }

        try
        {
            _cGuildUnionPort.LEngineAuthorAbsorb(kept, author);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cGuildUnionEnvoy, _cGuildUnionSettings, "Guild.MergeFailed", exception);
            return;
        }

        _cGuildUnionSeam(kept);
    }

    private bool LGuildUnionConfirm(long kept)
    {
        (string dropped, string held) =
            _cGuildUnionPort.LEngineUnionRead(_cGuildUnionAutograph.CDeskDraft.CDeskDraftTenure, kept);
        return _cGuildUnionEnvoy.CEnvoyUnionConfirm("Guild.MergeConfirm", dropped, held);
    }
}
