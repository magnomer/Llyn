# Llyn.Infrastructure.csproj

## `<PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3" Version="2.1.13" />`

Microsoft.Data.Sqlite 9.0.11 pins SQLitePCLRaw 2.1.10, which carries a high-severity SQLite advisory (GHSA-2m69-gcr7-jv3q).
This reference overrides the transitive bundle with a patched version.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of Infrastructure.
