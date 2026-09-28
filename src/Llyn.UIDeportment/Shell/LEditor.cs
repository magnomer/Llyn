using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LEditor
{
    internal LEditor(CEditor studio, LPhonologyPort phonology, LDraftPort drafts)
    {
        ArgumentNullException.ThrowIfNull(studio);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(drafts);

        LEditorStudio = studio;
        LEditorClip = new LClip(studio.CEditorDesk);
        LEditorNotation = new LNotation(studio.CEditorDesk);
        LEditorSounding = new LSounding(phonology, drafts);
        LEditorEsteem = new QEsteem(studio.CEditorDesk, studio.CEditorDisplay);
        LEditorTimbre = new QTimbre(this, phonology, studio.CEditorDisplay, LEditorSounding);
    }

    public CEditor LEditorStudio { get; }

    public LClip LEditorClip { get; }

    public LNotation LEditorNotation { get; }

    public LSounding LEditorSounding { get; }

    public QEsteem LEditorEsteem { get; }

    public QTimbre LEditorTimbre { get; }

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
