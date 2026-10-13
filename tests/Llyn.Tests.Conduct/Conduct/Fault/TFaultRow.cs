using System;
using System.Threading.Tasks;

namespace Llyn.Tests;

internal sealed record TFaultRow(
    string TFaultRowGate,
    string TFaultRowMember,
    string TFaultRowKey,
    Func<TFaultStage, Task<Func<Task>>> TFaultRowArrange);
