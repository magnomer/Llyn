using System;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal sealed class CPhonologyBundle
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

    internal LFanqiePort CPhonologyBundleFanqie { get; }

    internal LDiweiPort CPhonologyBundleDiwei { get; }

    internal LScriptPort CPhonologyBundleScript { get; }

    internal LLanguagePort CPhonologyBundleLanguage { get; }

    internal LReflexPort CPhonologyBundleReflex { get; }

    internal LParadigmPort CPhonologyBundleParadigm { get; }

    internal LSentencePort CPhonologyBundleSentence { get; }

    internal LStemPort CPhonologyBundleStem { get; }
}
