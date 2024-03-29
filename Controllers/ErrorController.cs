﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RemoteDirectoryList.Models;
using System;

namespace RemoteDirectoryList.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        public IActionResult DirectoryNotFound(string? requestedDirectoryPath)
        {
            _logger.LogError($"Someone is trying to access an unknown directory: {requestedDirectoryPath}");

            ViewData["tabs"] = Array.Empty<RootDirectoryTabModel>();

            return View();
        }
    }
}
