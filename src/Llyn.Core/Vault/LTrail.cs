namespace Llyn.Core;

public interface LTrail
{
    string? LTrailResolve(string root, string path);

    string? LTrailRelativeResolve(string root, string path);

    string LTrailNameRead(string path);

    bool LTrailRootCheck(string path);

    string LTrailNameNormalize(string name);
}
