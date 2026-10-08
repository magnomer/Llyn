using System;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CPhonologyBundle
{
    internal CPhonologyBundle(
        LFanqiePort fanqies,
        LDiweiPort diweis,
        LScriptPort scripts,
        LLanguagePort languages,
        LReflexPort reflexes,
        LParadigmPort paradigms,
        LSentencePort sentences,
        LStemPort stems)
    {
        ArgumentNullException.ThrowIfNull(fanqies);
        ArgumentNullException.ThrowIfNull(diweis);
        ArgumentNullException.ThrowIfNull(scripts);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(paradigms);
        ArgumentNullException.ThrowIfNull(sentences);
        ArgumentNullException.ThrowIfNull(stems);

        CPhonologyBundleFanqie = fanqies;
        CPhonologyBundleDiwei = diweis;
        CPhonologyBundleScript = scripts;
        CPhonologyBundleLanguage = languages;
        CPhonologyBundleReflex = reflexes;
        CPhonologyBundleParadigm = paradigms;
        CPhonologyBundleSentence = sentences;
        CPhonologyBundleStem = stems;
    }

    public LFanqiePort CPhonologyBundleFanqie { get; }

    public LDiweiPort CPhonologyBundleDiwei { get; }

    public LScriptPort CPhonologyBundleScript { get; }

    public LLanguagePort CPhonologyBundleLanguage { get; }

    public LReflexPort CPhonologyBundleReflex { get; }

    public LParadigmPort CPhonologyBundleParadigm { get; }

    public LSentencePort CPhonologyBundleSentence { get; }

    public LStemPort CPhonologyBundleStem { get; }
}
