using Microsoft.AspNetCore.Mvc.Filters;

namespace MyLocations.Web.Filters
{
    public class ModelStateValidationFilter(ILogger<ModelStateValidationFilter> logger) : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context) 
        {
            if (!context.ModelState.IsValid)
            {
                logger.LogError("The following keys were invalid");
                foreach (var modelState in context.ModelState)
                {
                    if (modelState.Value is not null && modelState.Value.Errors.Any())
                        logger.LogError($"Key: {modelState.Key} - Message : {modelState.Value!.Errors[0].ErrorMessage}");
                }
                throw new Exception("Model state invalid.");
            }
        }
    }
}
