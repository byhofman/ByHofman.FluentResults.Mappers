using FluentResults;

namespace ByHofman.FluentResults;

/// <summary>Extensions that read code and category information from a FluentResults error.</summary>
public static class ErrorExtensions
{
    /// <summary>
    /// Returns the error's code from the <see cref="MetadataKeys.Code"/> metadata entry,
    /// falling back to the error's type name.
    /// </summary>
    public static string GetCode(this IError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return error.Metadata.TryGetValue(MetadataKeys.Code, out var value) && value is string code
            ? code
            : error.GetType().Name;
    }

    /// <summary>Classifies the error based on the category marker interface it implements.</summary>
    public static ErrorCategory Categorize(this IError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return error switch
        {
            INotFoundError => ErrorCategory.NotFound,
            IValidationError => ErrorCategory.Validation,
            IUnauthorizedError => ErrorCategory.Unauthorized,
            IForbiddenError => ErrorCategory.Forbidden,
            IConflictError => ErrorCategory.Conflict,
            ITransientError => ErrorCategory.Transient,
            _ => ErrorCategory.Failure,
        };
    }
}
