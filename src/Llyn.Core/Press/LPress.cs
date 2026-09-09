using System.Threading.Tasks;

namespace Llyn.Core;

public interface LPress
{
    Task LPressSave(string html, string path);
}
