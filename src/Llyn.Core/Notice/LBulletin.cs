namespace Llyn.Core;

public sealed record LBulletin(LSubject LBulletinSubject, long LBulletinId)
{
    public bool LBulletinStored => LBulletinId > 0;

    public bool LBulletinMatch(LSubject subject)
    {
        return LBulletinSubject == subject;
    }
}
