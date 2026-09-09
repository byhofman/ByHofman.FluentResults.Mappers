namespace ByHofman.FluentResults;

/// <summary>Transport-agnostic classification of a FluentResults error.</summary>
public enum ErrorCategory
{
    /// <summary>An uncategorized failure.</summary>
    Failure,

    /// <summary>A requested resource does not exist.</summary>
    NotFound,

    /// <summary>The request was malformed or failed validation.</summary>
    Validation,

    /// <summary>Authentication is missing or invalid.</summary>
    Unauthorized,

    /// <summary>The caller is authenticated but not allowed.</summary>
    Forbidden,

    /// <summary>The request conflicts with the current state.</summary>
    Conflict,

    // Appended rather than inserted: the members carry implicit ordinals, and renumbering them
    // would change the meaning of any value already persisted or sent over a wire.

    /// <summary>The failure is temporary and the request is worth retrying.</summary>
    Transient,
}
