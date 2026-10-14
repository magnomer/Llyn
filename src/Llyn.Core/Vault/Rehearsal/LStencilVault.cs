using System.Collections.Generic;

namespace Llyn.Core;

public interface LStencilVault
{
    LStencil LStencilCreate(LStencil stencil);

    IReadOnlyList<LStencil> LStencilRead();

    LStencil? LStencilRead(long stencilId);

    void LStencilUpdate(LStencil stencil);
}
