using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeStencil : LStencilVault
{
    private readonly List<LStencil> _tVaultFakeStencils = [];
    private long _tVaultFakeNext = 100;

    public LStencil LStencilCreate(LStencil stencil)
    {
        LStencil created = stencil with { LStencilId = ++_tVaultFakeNext };
        _tVaultFakeStencils.Add(created);
        return created;
    }

    public IReadOnlyList<LStencil> LStencilRead() =>
        _tVaultFakeStencils.Where(static stencil => stencil.LStencilSaved).ToList();

    public LStencil? LStencilRead(long stencilId) =>
        _tVaultFakeStencils.Find(stencil => stencil.LStencilId == stencilId);

    public void LStencilUpdate(LStencil stencil)
    {
        int index = _tVaultFakeStencils.FindIndex(held => held.LStencilId == stencil.LStencilId);
        if (index < 0)
        {
            throw new InvalidOperationException($"No stencil carries the id '{stencil.LStencilId}'.");
        }

        _tVaultFakeStencils[index] = _tVaultFakeStencils[index] with
        {
            LStencilName = stencil.LStencilName,
            LStencilSlots = stencil.LStencilSlots,
        };
    }

    internal void TVaultFakeDelete(long stencilId)
    {
        _tVaultFakeStencils.RemoveAll(stencil => stencil.LStencilId == stencilId);
    }
}
