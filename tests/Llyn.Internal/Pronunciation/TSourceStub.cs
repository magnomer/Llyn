using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TSourceStub : LSource
{
    private readonly LAnswer _tSourceStubAnswer;

    internal TSourceStub(LAnswer answer)
    {
        _tSourceStubAnswer = answer;
    }

    public string LSourceName => "Stub";

    public Task<LAnswer> LSourceFind(string word, CancellationToken cancellation) =>
        Task.FromResult(_tSourceStubAnswer);
}
