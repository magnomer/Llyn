# CRegisterDraft.cs
Hash: `cc78c6b5ae887a4e`

## `public sealed record CRegisterDraft(long CRegisterDraftId, CStateWording CRegisterDraftName);`

One register of a card, as the card's register chips show it.

**Parameters**

- `CRegisterDraftId`: the stored register, zero for a fresh one.
- `CRegisterDraftName`: the register's name, worded for its chip.
