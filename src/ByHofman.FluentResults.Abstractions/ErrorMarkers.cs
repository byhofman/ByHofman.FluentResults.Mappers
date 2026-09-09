namespace ByHofman.FluentResults;

/// <summary>Marks an error as "resource not found" (maps to 404 / <c>NOT_FOUND</c>).</summary>
public interface INotFoundError;

/// <summary>Marks an error as a validation failure (maps to 400).</summary>
public interface IValidationError;

/// <summary>Marks an error as unauthenticated (maps to 401).</summary>
public interface IUnauthorizedError;

/// <summary>Marks an error as forbidden (maps to 403).</summary>
public interface IForbiddenError;

/// <summary>Marks an error as a state conflict (maps to 409).</summary>
public interface IConflictError;

/// <summary>
/// Marks a failure as transient — unreachable, timed out, or refused while overloaded (maps to
/// 503). Without this marker a transient failure falls through to the uncategorized default and
/// is reported as 400, which tells a caller not to retry something that is worth retrying.
/// </summary>
public interface ITransientError;
