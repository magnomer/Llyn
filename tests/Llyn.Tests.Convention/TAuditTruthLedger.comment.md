# TAuditTruthLedger.json
Hash: `93ec76fb1351ef63`

The driver audit's ceilings, by kind and then by repo-relative file.
Each ceiling is the count of that kind in that file.
Every kind may only fall.
Mismatching holds no rows, since it only informs while Demeanor has no code.
Once Demeanor holds a source, every mismatching hit fails.
A file missing from a kind has a ceiling of zero.
`TAuditRatchet` holds every ceiling against the committed copy, and a new generation baselines it afresh.
When a count falls, `AuditTruth_Ledger_MatchesHits` writes the lowered ledger under `temp/audit` to copy here.
