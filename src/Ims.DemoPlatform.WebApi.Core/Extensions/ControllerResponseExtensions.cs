using Ims.DemoPlatform.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Ims.DemoPlatform.WebApi.Core.Extensions;

public static class ControllerResponseExtensions
{
    public static ApiResponse<T> Success<T>(this ControllerBase _, T? data = default)
        => new(true, data);
    
    public static ApiResponse<object> Success(this ControllerBase _)
        => new(true, null);
    
    public static ApiResponse<object> Fail(this ControllerBase _, params string[] errors) =>
        new(false, null, errors.Length > 0 ? errors : null);
}