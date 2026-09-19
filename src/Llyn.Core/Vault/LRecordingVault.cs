using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LRecordingVault
{
    Task<string> LRecordingSave(LRecording recording, string word, string language, CancellationToken cancellation);

    Task<string> LRecordingPrepare(LRecording recording, CancellationToken cancellation);
}
