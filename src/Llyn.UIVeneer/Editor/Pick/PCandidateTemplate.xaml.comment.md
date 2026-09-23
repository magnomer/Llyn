# PCandidateTemplate.xaml.cs

## `public partial class PCandidateTemplate : ResourceDictionary`

This dictionary presents a candidate choice while the editor controls selection and card updates.

## `private readonly PEditor _pCandidateHost`

The host resolves candidate selection against the editor's current card.

## `internal PCandidateTemplate(PEditor host)`

The editor reference lets a shared candidate template return choices to their owner.

## `private void PCandidateHandle(object sender, MouseButtonEventArgs e)`

Candidate clicks go through the editor so selection updates the active draft consistently.
