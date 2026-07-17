using System.Collections.Concurrent;
using System.Linq.Expressions;
using FluentResults;
using HotChocolate;
using HotChocolate.Resolvers;

namespace ByHofman.FluentResults;

internal static class FluentResultsField
{
    private static readonly ConcurrentDictionary<Type, Func<object, object?>?> ValueAccessors = new();

    public static void Apply(IMiddlewareContext context, IResultBase result)
    {
        if (result.IsFailed)
        {
            foreach (var error in result.Errors)
                context.ReportError(BuildError(context, error));
            context.Result = null;
            return;
        }

        var accessor = ValueAccessors.GetOrAdd(result.GetType(), CreateValueAccessor);
        context.Result = accessor is null ? true : accessor(result);
    }

    public static Type UnwrapPayloadType(Type returnType)
    {
        var type = returnType;
        if (type.IsGenericType &&
            (type.GetGenericTypeDefinition() == typeof(Task<>) || type.GetGenericTypeDefinition() == typeof(ValueTask<>)))
        {
            type = type.GetGenericArguments()[0];
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var inner = type.GetGenericArguments()[0];
            return inner.IsValueType && Nullable.GetUnderlyingType(inner) is null
                ? typeof(Nullable<>).MakeGenericType(inner)
                : inner;
        }

        return typeof(bool?);
    }

    private static global::HotChocolate.IError BuildError(IMiddlewareContext context, global::FluentResults.IError error)
    {
        var builder = ErrorBuilder.New()
            .SetMessage(error.Message)
            .SetCode(error.GetCode())
            .SetPath(context.Path)
            .SetExtension("category", error.Categorize().ToString());

        foreach (var entry in error.Metadata)
        {
            if (entry.Key != MetadataKeys.Code)
                builder.SetExtension(entry.Key, entry.Value);
        }

        return builder.Build();
    }

    private static Func<object, object?>? CreateValueAccessor(Type resultType)
    {
        var property = resultType.GetProperty("Value");
        if (property is null)
            return null;

        var parameter = Expression.Parameter(typeof(object), "result");
        var body = Expression.Convert(
            Expression.Property(Expression.Convert(parameter, resultType), property),
            typeof(object));
        return Expression.Lambda<Func<object, object?>>(body, parameter).Compile();
    }
}
