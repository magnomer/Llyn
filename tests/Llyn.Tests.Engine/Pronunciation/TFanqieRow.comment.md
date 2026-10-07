# TFanqieRow.cs
Hash: `848333a15eb2c3e4`

## `public sealed class TFanqieRow`

The formatted columns of a fanqie row and the blocks the engine groups rows and images into.
The headword reading the blocks give is checked here too, since it reads the ranks off them.
Rows handed in out of order list ranked rows first by rank.
The rest follow by pack book, then reading, then id.
A book the pack does not declare follows the declared ones.
A book holding a ranked row leads its character's blocks, and each block keeps the declared row order.
