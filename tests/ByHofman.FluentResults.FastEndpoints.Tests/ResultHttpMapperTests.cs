using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ByHofman.FluentResults.FastEndpointsTests;

public class ResultHttpMapperTests
{
    private static readonly IServiceProvider Services = new ServiceCollection().AddLogging().BuildServiceProvider();

    private static async Task<(int Status, string Body)> ExecuteAsync(IResult httpResult)
    {
        var context = new DefaultHttpContext { RequestServices = Services };
        context.Response.Body = new MemoryStream();
        await httpResult.ExecuteAsync(context);
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        return (context.Response.StatusCode, await reader.ReadToEndAsync());
    }

    [Fact]
    public async Task Success_with_value_maps_to_200()
    {
        var (status, body) = await ExecuteAsync(Result.Ok(42).ToHttpResult());

        Assert.Equal(200, status);
        Assert.Contains("42", body);
    }

    [Fact]
    public async Task Empty_success_maps_to_204()
    {
        var (status, _) = await ExecuteAsync(Result.Ok().ToHttpResult());

        Assert.Equal(204, status);
    }

    [Fact]
    public async Task NotFound_error_maps_to_404_with_code()
    {
        var (status, body) = await ExecuteAsync(Result.Fail<int>(new TestNotFound()).ToHttpResult());

        Assert.Equal(404, status);
        Assert.Contains("not_found", body);
    }

    [Fact]
    public async Task Uncategorized_error_maps_to_400()
    {
        var (status, _) = await ExecuteAsync(Result.Fail<int>("boom").ToHttpResult());

        Assert.Equal(400, status);
    }

    [Fact]
    public async Task Conflict_error_maps_to_409()
    {
        var (status, _) = await ExecuteAsync(Result.Fail<int>(new TestConflict()).ToHttpResult());

        Assert.Equal(409, status);
    }

    [Fact]
    public async Task Mapped_success_projects_the_value()
    {
        var (status, body) = await ExecuteAsync(Result.Ok(3).ToHttpResult(value => new { doubled = value * 2 }));

        Assert.Equal(200, status);
        Assert.Contains("6", body);
    }
}

public sealed class TestNotFound() : CodedError("not_found", "missing"), INotFoundError;

public sealed class TestConflict() : CodedError("conflict", "duplicate"), IConflictError;
