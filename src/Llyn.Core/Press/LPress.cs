using System.Threading.Tasks;

namespace Llyn.Core;

public interface LPress
{
    Task LPressSave(string html, string path);

    Task LPressPrint(string html, LPressTicket ticket);
}
