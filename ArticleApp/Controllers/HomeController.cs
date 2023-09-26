using ArticleApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;

namespace ArticleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        public HomeController(ILogger logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("HomeController: /GET Index Called");
            return View();
        }

        public IActionResult TriggerError()
        {
            _logger.LogInformation("HomeController: /GET TriggerError Called");
            var exception = new Exception("This is a test exception.");
            _logger.LogError(exception, "An exception was thrown");
            _logger.LogError(exception.Message);
            throw exception;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            string requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var errorModel = new ErrorViewModel { RequestId = requestId };

            _logger.LogError($"Error occurred. Request Id: {errorModel.RequestId}");

            return View(errorModel);
        }
    }
}