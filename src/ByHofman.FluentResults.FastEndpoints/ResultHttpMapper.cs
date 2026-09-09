using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ByHofman.FluentResults;

/// <summary>Maps a FluentResults <see cref="Result"/> / <see cref="Result{TValue}"/> to an ASP.NET Core <see cref="IResult"/>.</summary>
public static class ResultHttpMapper
{
    /// <summary>Success maps to <c>204 No Content</c>; failure to an RFC 7807 problem response.</summary>
    public static IResult ToHttpResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess ? Results.NoContent() : ToProblem(result.Errors);
    }

    /// <summary>Success maps to <c>200 OK</c> with the value; failure to an RFC 7807 problem response.</summary>
    public static IResult ToHttpResult<TValue>(this Result<TValue> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result.Errors);
    }

    /// <summary>Success maps to <c>200 OK</c> with <paramref name="map"/> applied to the value; failure to an RFC 7807 problem response.</summary>
    public static IResult ToHttpResult<TSource, TValue>(this Result<TSource> result, Func<TSource, TValue> map)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(map);
        return result.IsSuccess ? Results.Ok(map(result.Value)) : ToProblem(result.Errors);
    }

    private static IResult ToProblem(IReadOnlyList<IError> errors)
    {
        var category = errors
            .Select(error => error.Categorize())
            .FirstOrDefault(c => c != ErrorCategory.Failure, ErrorCategory.Failure);

        var problem = new ProblemDetails
        {
            Status = StatusCodeFor(category),
            Title = TitleFor(category),
            Detail = errors.Count > 0 ? errors[0].Message : null,
        };

        problem.Extensions["errors"] = errors
            .Select(error => new ErrorDetail(error.GetCode(), error.Message))
            .ToArray();

        return Results.Problem(problem);
    }

    private static int StatusCodeFor(ErrorCategory category) => category switch
    {
        ErrorCategory.NotFound => StatusCodes.Status404NotFound,
        ErrorCategory.Validation => StatusCodes.Status400BadRequest,
        ErrorCategory.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorCategory.Forbidden => StatusCodes.Status403Forbidden,
        ErrorCategory.Conflict => StatusCodes.Status409Conflict,
        ErrorCategory.Transient => StatusCodes.Status503ServiceUnavailable,
        _ => StatusCodes.Status400BadRequest,
    };

    private static string TitleFor(ErrorCategory category) => category switch
    {
        ErrorCategory.NotFound => "Resource not found",
        ErrorCategory.Validation => "Validation failed",
        ErrorCategory.Unauthorized => "Unauthorized",
        ErrorCategory.Forbidden => "Forbidden",
        ErrorCategory.Conflict => "Conflict",
        ErrorCategory.Transient => "Service unavailable",
        _ => "Request failed",
    };

    private sealed record ErrorDetail(string Code, string Message);
}
