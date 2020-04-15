using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using O365StatusDashboard.Models.Configuration;
using O365StatusDashboard.Services;

namespace O365StatusDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ServiceHealthStatusService _serviceHealthStatusService;
        private readonly IOptions<CompanyConfiguration> _companyConfiguration;

        public HomeController(
            ILogger<HomeController> logger,
            ServiceHealthStatusService serviceHealthStatusService,
            IOptions<CompanyConfiguration> companyConfiguration)
        {
            _logger = logger;
            _serviceHealthStatusService = serviceHealthStatusService;
            _companyConfiguration = companyConfiguration;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
             ViewBag.CompanyName = _companyConfiguration.Value?.CompanyName;
             ViewBag.SupportEmail = _companyConfiguration.Value?.SupportEmail;
             ViewBag.SupportPhone = _companyConfiguration.Value?.SupportPhone;
             
             base.OnActionExecuting(context);
        }

        public async Task<IActionResult> Index()
        {
            var res = await _serviceHealthStatusService.GetCurrentStatusBlacklisted();
            return View(res);
        }
        
        [HttpGet("{workload}")]
        public async Task<IActionResult> Detail(string workload)
        {
            var res = await _serviceHealthStatusService.GetCurrentStatus(workload);
            return View(res);
        }
        
        public IActionResult Error()
        {
            return View();
        }
    }
}