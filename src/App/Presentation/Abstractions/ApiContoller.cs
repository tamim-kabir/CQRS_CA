using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Presentation.Abstractions;
[ApiController]
public abstract class ApiContoller : ControllerBase
{
    protected readonly ISender _sender;
    protected ApiContoller(ISender sender)
    {
        _sender = sender;
    }
    internal dynamic Result(Result command, [CallerMemberName] string memberName = "")
    {
        var info = new StackFrame(1, true)?.GetMethod();
        var member = ((TypeInfo)info.DeclaringType
                                    .DeclaringType)
                                    .DeclaredMethods
                                    .FirstOrDefault(x => x.Name.Equals(memberName));

        var httpMethodAttribute = member.GetCustomAttributes(false)
                                        .FirstOrDefault(x => x is HttpMethodAttribute);
        return httpMethodAttribute switch
        {
            HttpPostAttribute => command.IsSuccess ? Ok() : BadRequest(command.Error),
            _ => NoContent()
        };
    }
    internal dynamic Result<T>(Result<T> result, [CallerMemberName] string memberName = "") where T : class
    {
        var info = new StackFrame(1, true)?.GetMethod();
        var member = ((TypeInfo)info.DeclaringType
                                    .DeclaringType)
                                    .DeclaredMethods
                                    .FirstOrDefault(x => x.Name.Equals(memberName));

        var httpMethodAttribute = member.GetCustomAttributes(false)
                                        .FirstOrDefault(x => x is HttpMethodAttribute);
        return httpMethodAttribute switch
        {
            HttpPostAttribute => result.IsSuccess ? Ok() : BadRequest(result.Error),
            HttpGetAttribute => result.IsSuccess ? Ok(result.Value) : NotFound(result.Error),
            _ => NoContent()
        };
    }
}
