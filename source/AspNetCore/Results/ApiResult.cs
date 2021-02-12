using DotNetCore.Results;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNetCore.AspNetCore
{
    public sealed class ApiResult : IActionResult
    {
        private readonly IResult _result;

        private ApiResult(IResult result)
        {
            _result = result;
        }

        private ApiResult(object data)
        {
            _result = Result<object>.Success(data);
        }

        public static IActionResult Create(IResult result)
        {
            return new ApiResult(result);
        }

        public static IActionResult Create(object data)
        {
            return new ApiResult(data);
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            if (_result.Failed)
            {
                return new UnprocessableEntityObjectResult(_result.Message).ExecuteResultAsync(context);
            }

            if (_result.GetType().IsGenericType && _result.GetType().GetGenericTypeDefinition() == typeof(Result<>))
            {
                return new OkObjectResult((_result as dynamic)?.Data).ExecuteResultAsync(context);
            }

            return new OkObjectResult(_result.Message).ExecuteResultAsync(context);
        }
    }
}
