using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace cabme.webmvc.ActionFilters
{
    public class ValidationActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var errors = new Dictionary<string, IEnumerable<string>>();
                foreach (KeyValuePair<string, System.Web.Http.ModelBinding.ModelState> keyValue in actionContext.ModelState)
                {
                    errors[keyValue.Key] = keyValue.Value.Errors.Select(e => e.ErrorMessage);
                }

                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, errors);
            }
        }
    }
}