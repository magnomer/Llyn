using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LEditor
{
    internal LEditor(CEditor studio)
    {
        ArgumentNullException.ThrowIfNull(studio);

        LEditorStudio = studio;
    }

    public CEditor LEditorStudio { get; }

    public bool LEditorChanged => LEditorStudio.CEditorDesk.CDeskChanged;

    public bool LEditorStorable => LEditorStudio.CEditorDesk.CDeskStorable;

    public bool LEditorRunning => LEditorStudio.CEditorDesk.CDeskRunning;

    public long? LEditorEntry => LEditorStudio.CEditorDesk.CDeskStoredRead();

    public string LEditorLanguage => LEditorStudio.CEditorDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;

    public bool LEditorFlagged => LEditorStudio.CEditorDesk.CDeskTenure?.LTenureFlaggedCheck() ?? false;

    public bool LEditorReflexShown => LEditorStudio.CEditorDesk.CDeskTenure?.LTenureReflexCheck() ?? false;

    public bool LEditorMorphology => LEditorStudio.CEditorDisplay.LDisplaySound.LDisplayMorphologyRead();

    public IReadOnlyList<string> LEditorVarietyNames =>
        LEditorStudio.CEditorDesk.CDeskTenure?.LTenureVarietyNames ?? [];

    internal LTenure? LEditorTenure =>
        LEditorStudio.CEditorDesk.CDeskFilling ? null : LEditorStudio.CEditorDesk.CDeskTenure;
}
