using System;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LPressProof : IDisposable
{
    int LPressProofCount { get; }

    LPressPaper LPressProofRead(int page);

    Task<LPressImage> LPressProofDraw(int page, int resolution);
}
