namespace Llyn.Application;

public abstract record LRequest(long LRequestDraftId)
{
    public abstract string LRequestKey { get; }
}
