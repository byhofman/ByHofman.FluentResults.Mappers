using FluentResults;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace ByHofman.FluentResults;

/// <summary>Field descriptor extensions that add the FluentResults middleware.</summary>
public static class FluentResultsObjectFieldDescriptorExtensions
{
    /// <summary>
    /// Unwraps a FluentResults <c>Result</c> / <c>Result&lt;T&gt;</c> returned by the resolver: on success the
    /// inner value is surfaced, on failure each error is reported as a GraphQL error and the field resolves to null.
    /// </summary>
    public static IObjectFieldDescriptor UseFluentResults(this IObjectFieldDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        return descriptor.Use(next => async (IMiddlewareContext context) =>
        {
            await next(context);
            if (context.Result is IResultBase result)
                FluentResultsField.Apply(context, result);
        });
    }
}
