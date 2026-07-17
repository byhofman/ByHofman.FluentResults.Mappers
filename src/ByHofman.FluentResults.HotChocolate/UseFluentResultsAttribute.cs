using System.Reflection;
using System.Runtime.CompilerServices;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace ByHofman.FluentResults;

/// <summary>
/// Applies the FluentResults field middleware to a resolver and rewrites the GraphQL field type
/// from <c>Result&lt;T&gt;</c> to the unwrapped payload <c>T</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public sealed class UseFluentResultsAttribute : ObjectFieldDescriptorAttribute
{
    public UseFluentResultsAttribute([CallerLineNumber] int order = 0) => Order = order;

    protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo? member)
    {
        descriptor.UseFluentResults();

        if (member is MethodInfo method)
        {
            var payload = FluentResultsField.UnwrapPayloadType(method.ReturnType);
            descriptor.Extend().OnBeforeCreate((completionContext, definition) =>
                definition.Type = completionContext.TypeInspector.GetTypeRef(payload, TypeContext.Output));
        }
    }
}
