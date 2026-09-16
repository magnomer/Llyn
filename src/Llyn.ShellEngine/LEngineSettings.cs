using System;
using System.Collections.Generic;
using System.IO;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LSettings LEngineSettingsRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineSettings;
        }
    }

    public void LEngineLocalizationSave(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        LEngineSettingsChange(settings => settings with { LSettingsLocalization = language });
    }

    public void LEngineWindowSave(LWindowState window)
    {
        ArgumentNullException.ThrowIfNull(window);
        LEngineSettingsChange(settings => settings with { LSettingsWindow = window });
    }

    public void LEngineVolumeSave(double volume)
    {
        double level = Math.Clamp(volume, 0, 1);
        LEngineSettingsChange(settings => settings with { LSettingsVolume = level });
    }

    public void LEngineRespellingSave(bool respelled)
    {
        LEngineSettingsChange(settings => settings with { LSettingsRespelled = respelled });
        LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
    }

    public bool LEngineRespellingCheck(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineSettings.LSettingsRespelled
                && language.Trim().Length > 0
                && LEngineLanguageLoad(language).LLanguageRespellings.Count > 0;
        }
    }

    public bool LEnginePhonemicCheck(string language)
    {
        return language.Trim().Length > 0 && LEngineLanguageLoad(language).LLanguagePhonemic;
    }

    public void LEngineEpithetSave(bool epithet)
    {
        LEngineSettingsChange(settings => settings with { LSettingsEpithet = epithet });
    }

    public void LEngineTallySave(bool respelled)
    {
        LEngineSettingsChange(settings => settings with { LSettingsTally = respelled });
    }

    public void LEngineFrequencySave(bool frequency)
    {
        LEngineSettingsChange(settings => settings with { LSettingsFrequency = frequency });
    }

    public void LEngineMorphologySave(bool morphology)
    {
        lock (_lEngineGate)
        {
            LEngineSettingsChange(settings => settings with { LSettingsMorphology = morphology });
            if (!morphology)
            {
                LEngineInflectionClear();
            }
        }
    }

    public void LEngineLayoutSave(IEnumerable<LLayout> layout)
    {
        ArgumentNullException.ThrowIfNull(layout);

        lock (_lEngineGate)
        {
            Dictionary<string, LLayout> merged = new(StringComparer.Ordinal);

            foreach (LLayout tab in _lEngineSettings.LSettingsLayout ?? [])
            {
                merged[tab.LLayoutTab] = tab;
            }

            foreach (LLayout tab in layout)
            {
                merged[tab.LLayoutTab] = merged.TryGetValue(tab.LLayoutTab, out LLayout? held)
                    ? held with
                    {
                        LLayoutLeft = tab.LLayoutLeft ?? held.LLayoutLeft,
                        LLayoutMiddle = tab.LLayoutMiddle ?? held.LLayoutMiddle,
                        LLayoutOrder = tab.LLayoutOrder ?? held.LLayoutOrder,
                        LLayoutFilter = tab.LLayoutFilter ?? held.LLayoutFilter,
                    }
                    : tab;
            }

            List<LLayout> list = [.. merged.Values];
            LEngineSettingsChange(settings => settings with { LSettingsLayout = list });
        }
    }

    public void LEngineLayoutReset()
    {
        lock (_lEngineGate)
        {
            List<LLayout> list = [];

            foreach (LLayout tab in _lEngineSettings.LSettingsLayout ?? [])
            {
                list.Add(tab with { LLayoutLeft = null, LLayoutMiddle = null });
            }

            LEngineSettingsChange(settings => settings with { LSettingsLayout = list });
        }
    }

    public void LEngineLinkedSave(bool linked)
    {
        LEngineSettingsChange(settings => settings with { LSettingsLinked = linked });
    }

    private void LEngineSettingsChange(Func<LSettings, LSettings> change)
    {
        lock (_lEngineGate)
        {
            LSettings changed = change(_lEngineSettings);
            if (changed == _lEngineSettings)
            {
                return;
            }

            _lEngineSettings = changed;
            LEngineSettingsSave();
        }
    }

    private void LEngineSettingsSave()
    {
        try
        {
            LSettingsLoader.LSettingsLoaderSave(_lEngineWorkspace, _lEngineSettings);
        }
        catch (IOException exception)
        {
            LAuditWriter.LAuditWriterRecord(_lEngineWorkspace, exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            LAuditWriter.LAuditWriterRecord(_lEngineWorkspace, exception);
        }
    }
}
