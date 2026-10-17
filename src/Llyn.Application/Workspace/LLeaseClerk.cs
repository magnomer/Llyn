using System;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LLeaseClerk
{
    private readonly LLease _lLeaseClerkLease;

    public LLeaseClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lLeaseClerkLease = rig.LRigLease;
    }

    public void LLeaseClerkOpen()
    {
        _lLeaseClerkLease.LLeaseOpen();
    }

    public void LLeaseClerkClose()
    {
        _lLeaseClerkLease.LLeaseClose();
    }

    public bool LLeaseClerkMatch(LLeaseClerk? other)
    {
        return other is not null && ReferenceEquals(_lLeaseClerkLease, other._lLeaseClerkLease);
    }
}
