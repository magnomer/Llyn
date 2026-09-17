namespace Llyn.Core;

public abstract record LRequest(long LRequestDraftId)
{
    public virtual string LRequestKey => GetType().Name;
}
