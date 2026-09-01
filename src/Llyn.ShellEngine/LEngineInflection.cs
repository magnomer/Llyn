using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The inflection half of the engine: the inflected forms an Entry owns, each with the ordered
/// grammatical features that say what form it is. Unlike a Tag or an Example, an inflection is not
/// independent data — it belongs to its Entry, is identified by its place in that Entry's list, and
/// goes when the Entry goes. That is why these seams are keyed by the Entry and a position rather than
/// by an id of their own.
/// <para>
/// Writing the whole list and appending to it are separate seams because they answer different
/// questions. <see cref="LEngineInflectionSet"/> makes the stored list the list it is handed, which is
/// what an editor showing every form does; <see cref="LEngineInflectionAppend"/> adds to the end
/// without reading what is there, which is what a source contributing forms does. Neither is the other
/// with an argument.
/// </para>
/// <para>
/// The feature ids an inflection carries are resolved for display through the morphology vocabulary,
/// not stored as words: a form knows it is plural, and what "plural" is called in a language is a fact
/// of that language's pack.
/// </para>
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Reads the inflected forms of the Entry identified by <paramref name="entryId"/>, in stored
    /// order, each with its features in theirs.
    /// </summary>
    public IReadOnlyList<LInflection> LEngineInflectionRead(string entryId)
    {
        return new LInflectionArchive(_lEngineDatabase).LInflectionRead(entryId);
    }

    /// <summary>
    /// Makes the stored inflections of the Entry identified by <paramref name="entryId"/> exactly
    /// <paramref name="inflections"/>, in the order given: an empty list clears them.
    /// </summary>
    public void LEngineInflectionSet(string entryId, IReadOnlyList<LInflection> inflections)
    {
        ArgumentNullException.ThrowIfNull(inflections);
        new LInflectionArchive(_lEngineDatabase).LInflectionSet(entryId, inflections);
    }

    /// <summary>
    /// Appends <paramref name="inflections"/> to the end of the Entry's list, leaving the forms already
    /// stored where they are.
    /// </summary>
    public void LEngineInflectionAppend(string entryId, IReadOnlyList<LInflection> inflections)
    {
        ArgumentNullException.ThrowIfNull(inflections);
        new LInflectionArchive(_lEngineDatabase).LInflectionAppend(entryId, inflections);
    }

    /// <summary>
    /// Moves the inflection at <paramref name="position"/> in the Entry's list to
    /// <paramref name="target"/>, renumbering the list so the positions stay contiguous.
    /// </summary>
    public void LEngineInflectionMove(string entryId, int position, int target)
    {
        new LInflectionArchive(_lEngineDatabase).LInflectionMove(entryId, position, target);
    }

    /// <summary>
    /// Deletes the inflection at <paramref name="position"/> in the Entry's list with its features, and
    /// renumbers the forms after it so the list stays contiguous.
    /// </summary>
    public void LEngineInflectionDelete(string entryId, int position)
    {
        new LInflectionArchive(_lEngineDatabase).LInflectionDelete(entryId, position);
    }
}
