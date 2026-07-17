using ByHofman.FluentResults;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FR = global::FluentResults;

namespace ByHofman.FluentResults.GraphQlTests;

public class UseFluentResultsTests
{
    private static async Task<string> ExecuteAsync(string query)
    {
        var executor = await new ServiceCollection()
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .BuildRequestExecutorAsync();

        var result = await executor.ExecuteAsync(query);
        return result.ToJson();
    }

    [Fact]
    public async Task Success_surfaces_the_inner_value()
    {
        var json = await ExecuteAsync("{ ok }");

        Assert.Contains("hello", json);
        Assert.DoesNotContain("\"errors\"", json);
    }

    [Fact]
    public async Task Failure_reports_a_graphql_error_with_the_code()
    {
        var json = await ExecuteAsync("{ missing }");

        Assert.Contains("\"errors\"", json);
        Assert.Contains("not_found", json);
    }
}

public class Query
{
    [UseFluentResults]
    public FR.Result<string> Ok() => FR.Result.Ok("hello");

    [UseFluentResults]
    public FR.Result<string> Missing() => FR.Result.Fail(new TestNotFound());
}

public sealed class TestNotFound() : CodedError("not_found", "nope"), INotFoundError;
