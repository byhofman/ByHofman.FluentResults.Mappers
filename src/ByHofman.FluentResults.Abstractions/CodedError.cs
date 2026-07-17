using FluentResults;

namespace ByHofman.FluentResults;

/// <summary>A FluentResults <see cref="Error"/> that carries a machine-readable code in its metadata.</summary>
public abstract class CodedError : Error
{
    protected CodedError(string code, string message) : base(message)
    {
        ArgumentException.ThrowIfNullOrEmpty(code);
        Code = code;
        Metadata[MetadataKeys.Code] = code;
    }

    /// <summary>The machine-readable error code.</summary>
    public string Code { get; }
}
