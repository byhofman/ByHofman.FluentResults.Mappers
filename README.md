# ByHofman.FluentResults.Mappers

Maps [FluentResults](https://github.com/altmann/FluentResults) `Result` / `Result<T>` onto web
framework responses, so your application and domain layers return `Result` and the transport layer
translates it — no `try/catch`, no status codes leaking into handlers.

Three packages live here:

| Package | What it does |
|---------|--------------|
| **ByHofman.FluentResults.Abstractions** | Framework-agnostic core: error category markers, coded-error base, code + category extensions. |
| **ByHofman.FluentResults.FastEndpoints** | Maps a `Result` to a FastEndpoints / ASP.NET Core HTTP response (`200`/`204`/`404`/`400` …). |
| **ByHofman.FluentResults.HotChocolate** | Field middleware that unwraps a `Result` in a GraphQL resolver and reports failures as GraphQL errors. |

Both mapper packages depend on `Abstractions` and are versioned together.

## Error model

Errors carry meaning through **marker interfaces** on your FluentResults `Error` types, so the
mappers stay decoupled from your domain:

```csharp
using ByHofman.FluentResults;

public sealed class NotFoundError(string entity, object key)
    : CodedError("not_found", $"{entity} '{key}' was not found."), INotFoundError;
```

`INotFoundError` → 404 / `NOT_FOUND`, `IValidationError` → 400, `IUnauthorizedError` → 401,
`IForbiddenError` → 403, `IConflictError` → 409, `ITransientError` → 503. Anything else is a
generic failure mapped to 400 — so mark a failure that is worth retrying, or a caller is told
not to.

See each package's README for usage.

## License

MIT
