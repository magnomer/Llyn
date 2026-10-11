using System;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LPressBoard : IAsyncDisposable
{
    Task<LPressProof> LPressBoardDraw(LPressSetting setting);

    Task<LPressStatus> LPressBoardPrint(LPressSetting setting);

    Task LPressBoardExport(LPressSetting setting, string path);

    Task LPressBoardShow();
}
