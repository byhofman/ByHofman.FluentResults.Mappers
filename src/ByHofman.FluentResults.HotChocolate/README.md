# ByHofman.FluentResults.HotChocolate

HotChocolate field middleware that unwraps a FluentResults `Result` / `Result<T>` returned by a
resolver: on success the inner value is surfaced, on failure each error is reported as a GraphQL
error carrying the FluentResults code, category and metadata.

```csharp
using ByHofman.FluentResults;

public sealed class WeatherMutation
{
    [UseFluentResults]
    public Task<Result<ForecastDto>> AddForecast(AddForecastInput input, [Service] IDispatcher dispatcher)
        => dispatcher.SendAsync(new AddForecastCommand(input.Date, input.TemperatureC));
}
```

The field's GraphQL type becomes the unwrapped payload (`ForecastDto` here); a failed `Result`
resolves the field to `null` and adds a GraphQL error whose `code` extension is the error's code
and whose `category` extension is its `ErrorCategory`.

Prefer the fluent form when configuring types by hand:

```csharp
descriptor
    .Field("addForecast")
    .Resolve(/* ... returns Result<ForecastDto> ... */)
    .UseFluentResults();
```

Error codes and categories come from `ByHofman.FluentResults.Abstractions` — give your errors
meaning by deriving from `CodedError` and implementing the category markers (`INotFoundError`, …).

## License

MIT
