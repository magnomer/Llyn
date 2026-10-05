namespace Llyn.Core;

public interface LWarrant
{
    string LWarrantHide(string token);

    string? LWarrantRestore(string hidden);
}
