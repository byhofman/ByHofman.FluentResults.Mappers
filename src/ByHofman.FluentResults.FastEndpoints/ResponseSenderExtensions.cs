using FastEndpoints;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace ByHofman.FluentResults;

/// <summary>Sends a FluentResults <see cref="Result"/> as an HTTP response from a FastEndpoints endpoint.</summary>
public static class ResponseSenderExtensions
{
    /// <summary>Sends <c>204</c> on success, or a problem response for the failure.</summary>
    public static Task ResultAsync(this IResponseSender sender, Result result)
    {
        ArgumentNullException.ThrowIfNull(sender);
        return result.ToHttpResult().ExecuteAsync(sender.HttpContext);
    }

    /// <summary>Sends <c>200</c> with the value on success, or a problem response for the failure.</summary>
    public static Task ResultAsync<TValue>(this IResponseSender sender, Result<TValue> result)
    {
        ArgumentNullException.ThrowIfNull(sender);
        return result.ToHttpResult().ExecuteAsync(sender.HttpContext);
    }

    /// <summary>Sends <c>200</c> with <paramref name="map"/> applied on success, or a problem response for the failure.</summary>
    public static Task ResultAsync<TSource, TValue>(this IResponseSender sender, Result<TSource> result, Func<TSource, TValue> map)
    {
        ArgumentNullException.ThrowIfNull(sender);
        return result.ToHttpResult(map).ExecuteAsync(sender.HttpContext);
    }
}
