# Llyn.Application.csproj

Builds application workflows over the Core model.

## Project ring

References Core and has no dependency on shell or infrastructure projects.
Nullable analysis is enabled and implicit global usings are disabled.

## `<WarningsAsErrors>CA1416</WarningsAsErrors>`

A Windows-only call turns the build red, so the platform stays out of Application.
