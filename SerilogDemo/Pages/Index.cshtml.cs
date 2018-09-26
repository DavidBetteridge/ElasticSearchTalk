using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SerilogDemo.Pages
{
    public class IndexModel : PageModel
    {
        readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
            // A simple log entry
            _logger.LogInformation("This is message from this great demo");

            // A structured log entry
            var module = "HighValueAccounts";
            var user = "David";
            var machine = "pc01";
            _logger.LogInformation("Unauthorised access attempt to {Module} by user {User} on machine {Machine}",
                                   module, user, machine);
        }
    }
}
