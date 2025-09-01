using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Financecontrol.WebApi.Filters;

public class CustomExceptionFilter : IExceptionFilter
{
    private readonly ITempDataDictionaryFactory _tempDataFactory;

    public CustomExceptionFilter(ITempDataDictionaryFactory tempDataFactory)
    {
        _tempDataFactory = tempDataFactory;
    }

    public void OnException(ExceptionContext context)
    {
        var controller = (string?)context.RouteData.Values["controller"] ?? "Home";
        var action     = (string?)context.RouteData.Values["action"] ?? "Index";

        var tempData = _tempDataFactory.GetTempData(context.HttpContext);
        tempData["Erro"] = context.Exception.Message;

        context.Result = new RedirectToActionResult(action, controller, routeValues: null);
        context.ExceptionHandled = true;
    }
}
