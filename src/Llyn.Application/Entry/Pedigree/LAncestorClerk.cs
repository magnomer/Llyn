using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LAncestorClerk
{
    private readonly LAncestorVault _lAncestorClerkAncestors;
    private readonly LLanguageClerk _lAncestorClerkLanguages;

    public LAncestorClerk(LRig rig, LLanguageClerk languages)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        _lAncestorClerkAncestors = rig.LRigLexicon.LRigLexiconAncestors;
        _lAncestorClerkLanguages = languages;
    }

    public IReadOnlyList<LAncestorRelation> LAncestorRelationRead()
    {
        return _lAncestorClerkAncestors.LAncestorRelationRead()
            .OrderByDescending(static relation => relation.LAncestorRelationBuiltin)
            .ToList();
    }

    public LAncestorRelation LAncestorRelationCreate(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string written = name.Trim();
        return LAncestorRelationRead().FirstOrDefault(relation => string.Equals(
                relation.LAncestorRelationName, written, StringComparison.OrdinalIgnoreCase))
            ?? _lAncestorClerkAncestors.LAncestorRelationCreate(written);
    }

    public int? LAncestorRelationDelete(long relationId)
    {
        LAncestorRelation? held = _lAncestorClerkAncestors.LAncestorRelationRead()
            .FirstOrDefault(relation => relation.LAncestorRelationId == relationId);
        if (held is null || held.LAncestorRelationBuiltin)
        {
            return null;
        }

        return _lAncestorClerkAncestors.LAncestorRelationDelete(relationId) ? held.LAncestorRelationUsage : null;
    }

    public IReadOnlyList<LAncestorLanguage> LAncestorLanguageRead()
    {
        List<LAncestorLanguage> languages = [];
        foreach (string language in _lAncestorClerkLanguages.LLanguageClerkRead())
        {
            languages.Add(new LAncestorLanguage(language));
        }

        foreach (LAncestorTongue tongue in _lAncestorClerkAncestors.LAncestorTongueRead())
        {
            languages.Add(new LAncestorLanguage(
                tongue.LAncestorTongueName, tongue.LAncestorTongueId, tongue.LAncestorTongueUsage));
        }

        return languages;
    }

    public LAncestorLanguage LAncestorTongueCreate(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string written = name.Trim();
        LAncestorLanguage? held = LAncestorLanguageRead().FirstOrDefault(language => string.Equals(
            language.LAncestorLanguageName, written, StringComparison.OrdinalIgnoreCase));
        if (held is not null)
        {
            return held;
        }

        LAncestorTongue created = _lAncestorClerkAncestors.LAncestorTongueCreate(written);
        return new LAncestorLanguage(
            created.LAncestorTongueName, created.LAncestorTongueId, created.LAncestorTongueUsage);
    }

    public int? LAncestorTongueDelete(long tongueId)
    {
        LAncestorTongue? held = _lAncestorClerkAncestors.LAncestorTongueRead()
            .FirstOrDefault(tongue => tongue.LAncestorTongueId == tongueId);
        if (held is null)
        {
            return null;
        }

        return _lAncestorClerkAncestors.LAncestorTongueDelete(tongueId) ? held.LAncestorTongueUsage : null;
    }
}
