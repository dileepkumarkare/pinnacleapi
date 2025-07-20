using DevExpress.XtraPivotGrid.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Pinnacle.Models;
using System.Text.Json;

namespace Pinnacle.Helpers
{
    public class AuditLogFilter : IActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditLogFilter> _logger;
        PinnacleDbContext _db = new PinnacleDbContext();
        MasterModel masterModel = new MasterModel();
        public AuditLogFilter(IHttpContextAccessor httpContextAccessor, ILogger<AuditLogFilter> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            JwtStatus jwtData = (JwtStatus)tokenStatus.data;
            var audit = new AuditLog
            {
                UserName = jwtData?.UserName ?? "Anonymous",
                Controller = context.RouteData.Values["controller"]?.ToString(),
                Action = context.RouteData.Values["action"]?.ToString(),
                Method = httpContext.Request.Method,
                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                PrevData = JsonSerializer.Serialize(context.ActionArguments)
            };
            if (!audit.Action.Contains("Get"))
            {
                _db.AuditLog.Add(audit);
                _db.SaveChanges();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }

}
