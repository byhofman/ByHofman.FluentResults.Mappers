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
