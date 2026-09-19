namespace Llyn.Core;

public interface LUsher
{
    bool LUsherPathExist(string? path);

    void LUsherPathDelete(string path);
}
