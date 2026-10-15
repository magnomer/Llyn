using System;

namespace Llyn.Core;

public sealed record LBulletin(LSubject LBulletinSubject, long LBulletinId, Exception? LBulletinRefusal)
{
    public bool LBulletinStored => LBulletinId > 0;
}
