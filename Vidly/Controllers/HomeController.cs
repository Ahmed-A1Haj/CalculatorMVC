using System.Diagnostics;
using System.Linq.Expressions;
using Arethmic_expressions_processor;
using Microsoft.AspNetCore.Mvc;
using Vidly.Interfaces;
using Vidly.Models;

namespace Vidly.Controllers
{
    public class HomeController : Controller
    {
        private IExpressionHandler _handler;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, IExpressionHandler handler)
        {
            _logger = logger;
            _handler = handler;
        }

        public IActionResult Index()
        {

            return View();
        }


        [HttpPost]
        public IActionResult Index([FromBody] string expression)
        {
            if (string.IsNullOrWhiteSpace(expression)) 
            {
                return Json(new
                {
                    sucess = false,
                    error = "Expression Is empty"
                });
            }
            try
            {
                var postfix = _handler.InfixToPostfix(expression);
                var result = _handler.EvaluatePostfix(postfix);
                return Json(new 
                {
                    success = true,
                    result = result
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    error = e.Message
                });
            };

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
