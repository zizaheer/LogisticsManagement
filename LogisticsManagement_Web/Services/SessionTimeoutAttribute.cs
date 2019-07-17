using LogisticsManagement_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Services
{
    /// <summary>
    /// This class is under development to handle the session timeout from a single class,
    /// in stead of current implementation where every action of controller has a ValidateSession method.
    /// This class is not yet used. TODO
    /// 
    /// </summary>
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        SessionData sessionData = new SessionData();
        ISession _httpContext;
        public SessionTimeoutAttribute(ISession httpContext)
        {
            _httpContext = httpContext;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_httpContext.GetString("SessionData") != null)
            {
                sessionData = JsonConvert.DeserializeObject<SessionData>(_httpContext.GetString("SessionData"));
                if (sessionData == null)
                {
                    filterContext.Result = new RedirectResult("Login/Index");
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }

    }

}
