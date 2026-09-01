# TAuditName.cs

## Inline notes

### `bool anyTestPrefixed = candidates.Any(candidate =>`

The test suite either names every test method descriptively without a prefix (all exempt) or commits to the T prefix on every one. A single T-prefixed test method flips the whole suite into the prefix-required mode.

### `if (!anyTestPrefixed)`

A test method carries a free-form scenario description, never an object base or verb. While no test method uses the T prefix the whole suite is descriptive and exempt; once any test method adopts T, every test method must carry it.

### `return "missing required prefix";`

A codebase-owned name that survived the external/generated/framework-contract filters but carries no prefix is a violation, not something to skip.

### `private static bool TAuditContractCheck(SyntaxNode node, string name)`

A member whose name is a contract member of a framework interface the nearest enclosing type declares is externally fixed by that interface — the same reason an explicit interface implementation is exempt, applied to the implicit form. For a partial type the interface may be declared in another fragment this file cannot see, so a contract-member name is accepted on the name alone.
