using System;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BEditorChronicle
{
    private readonly CDeskChronicle _bEditorChronicleGate;

    private readonly Func<BField?> _bEditorChronicleField;

    private bool _bEditorChroniclePending;

    internal BEditorChronicle(CDesk desk, CEntry entry, Func<BField?> field, Action reopen)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(reopen);

        _bEditorChronicleGate = desk.CDeskChronicle;
        _bEditorChronicleField = field;
        entry.CEntryDraftChanged += _ =>
        {
            if (_bEditorChroniclePending && field() is not null)
            {
                reopen();
            }

            _bEditorChroniclePending = false;
        };
        desk.CDeskStarted +=() => _bEditorChroniclePending = false;
    }

    internal (bool CDeskBackward, bool CDeskForward) BEditorChronicleState =>
        _bEditorChronicleGate.CDeskChronicleState;

    internal void BEditorChronicleUndo()
    {
        if (_bEditorChronicleField() is BField open)
        {
            _ = open.BFieldPendingApply(null);
            _bEditorChroniclePending = true;
        }

        _bEditorChronicleGate.CDeskChronicleUndo();
    }

    internal void BEditorChronicleRedo()
    {
        if (_bEditorChronicleField() is BField open)
        {
            _ = open.BFieldPendingApply(null);
            _bEditorChroniclePending = true;
        }

        _bEditorChronicleGate.CDeskChronicleRedo();
    }
}
