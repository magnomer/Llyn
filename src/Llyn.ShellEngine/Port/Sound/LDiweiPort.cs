using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LDiweiPort
{
    (long, bool)? LEngineDiweiFind(string language, string kind, string key);

    IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, long? chosen, bool final);

    LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize);

    long LEngineDiweiResolve(long? id, string character);

    IReadOnlyList<LVistaRow> LEngineXiaoyunFind(long? chosen, LVista onset, LVista rime, LVista vista);

    string? LEngineDiweiRead(bool initial, string key);
}
