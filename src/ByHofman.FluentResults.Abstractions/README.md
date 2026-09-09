# ByHofman.FluentResults.Abstractions

Framework-agnostic core shared by the ByHofman FluentResults mappers.

- **Category markers** — `INotFoundError`, `IValidationError`, `IUnauthorizedError`,
  `IForbiddenError`, `IConflictError`, `ITransientError`. Implement them on your `Error` types to
  give errors meaning.
- **`CodedError`** — an `Error` base that carries a machine-readable `Code`.
- **Extensions** — `IError.GetCode()` and `IError.Categorize()`.

```csharp
using ByHofman.FluentResults;

public sealed class NotFoundError(string entity, object key)
    : CodedError("not_found", $"{entity} '{key}' was not found."), INotFoundError;

// later
error.GetCode();      // "not_found"
error.Categorize();   // ErrorCategory.NotFound
```

## License

MIT
