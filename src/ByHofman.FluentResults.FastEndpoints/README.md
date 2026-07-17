# ByHofman.FluentResults.FastEndpoints

Maps a FluentResults `Result` / `Result<T>` onto a FastEndpoints (or minimal-API) HTTP response.

```csharp
public sealed class GetForecast(IDispatcher dispatcher) : EndpointWithoutRequest<ForecastDto>
{
    public override void Configure() => Get("/forecasts/{id}");

    public override async Task HandleAsync(CancellationToken ct)
    {
        Result<ForecastDto> result = await dispatcher.QueryAsync(new GetForecast(Route<Guid>("id")), ct);
        await Send.ResultAsync(result);   // 200 with the value, or 404 / 400 / 409 ... from the error
    }
}
```

Failures map by category (see `ByHofman.FluentResults.Abstractions`): `NotFound` → 404, `Validation`
→ 400, `Unauthorized` → 401, `Forbidden` → 403, `Conflict` → 409, everything else → 400 (configurable).
The body is an RFC 7807 `ProblemDetails` carrying the error code and messages.

You can also convert without sending — useful in minimal APIs:

```csharp
IResult http = result.ToHttpResult();
```

## License

MIT
