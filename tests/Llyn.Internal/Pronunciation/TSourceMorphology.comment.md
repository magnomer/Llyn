# TSourceMorphology.cs

## `public sealed class TSourceMorphology`

Covers the generic source reading inflected forms from one fetched body, as a pack's `morphology` list declares them.
Three regex readings keyed on morphology value codes come back tagged `"5"`, `"6"` and `"2"` with their spelling untouched.
A missed confirm guard is a blank answer that was reached.
A server failure is a lost answer.

## Inline notes

### `private const string TSourceMorphologyBody =`

Three form-of blocks in the shape the Wiktionary REST HTML writes them, one per inflected form.
The class names the kind of form and the anchor text is the form itself.
