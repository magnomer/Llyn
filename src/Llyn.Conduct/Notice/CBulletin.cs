using System;

namespace Llyn.Conduct;

public sealed record CBulletin(long CBulletinId, bool CBulletinStored, Exception? CBulletinRefusal = null);
