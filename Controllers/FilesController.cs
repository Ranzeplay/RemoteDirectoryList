using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RemoteDirectoryList.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteDirectoryList.Controllers
{
    public class FilesController : Controller
    {
        private readonly ILogger<FilesController> _logger;
        private readonly AppSettings _appSettings;

        public FilesController(ILogger<FilesController> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public IActionResult Directory(string path = "", string tabId = null)
        {
            if(tabId == null)
            {
                tabId = _appSettings.DefaultTabId;
            }

            // Adjust root directory to current tab
            var tabs = _appSettings.RootDirectoryTabs.ToArray();
            var tab = tabs.FirstOrDefault(t => t.Id == tabId);
            if(tab == null)
            {
                return RedirectToAction("DirectoryNotFound", "Error", new { requestedDirectoryPath = path });
            }

            var currentRootDirectory = tab.AbsolutePath;

            path = path.TrimStart('/').TrimStart('\\');

            var model = new DirectoryViewModel
            {
                DirectoryPath = Path.Combine(currentRootDirectory, path),
                Files = new List<EntryViewModel>(),
                TabId = tabId
            };

            model.IsRootDirectory = model.DirectoryPath == currentRootDirectory;

            var directory = new DirectoryInfo(model.DirectoryPath);
            if(directory.Parent != null)
            {
                model.ParentDirectoryPath = directory.Parent.FullName.Replace(currentRootDirectory, "");
            }
            else
            {
                model.ParentDirectoryPath = directory.FullName.Replace(currentRootDirectory, "");
            }

            if (!directory.Exists)
            {
                _logger.LogWarning("Someone is reading a non-existing directory");
                return RedirectToAction("DirectoryNotFound", "Error", new { requestedDirectoryPath = path });
            }

            #region List directory entries
            // List all directories
            directory.GetDirectories().ToList().ForEach(file =>
            {
                var fileModel = new EntryViewModel
                {
                    LastModifyTime = file.LastWriteTimeUtc,
                    Name = file.Name,
                    Size = new FileSize { Byte = 0 },
                    IsDirectory = true
                };

                model.Files.Add(fileModel);
            });

            // List all files
            directory.GetFiles().ToList().ForEach(file =>
            {
                var fileModel = new EntryViewModel
                {
                    LastModifyTime = file.LastWriteTimeUtc,
                    Name = file.Name,
                    Size = new FileSize { Byte = file.Length },
                    IsDirectory = false
                };

                model.Files.Add(fileModel);
            });
            #endregion

            // Put all tabs into view
            ViewData["Tabs"] = tabs;

            return base.View(model);
        }

        public IActionResult Download(string path, string tabId = null)
        {
            // Adjust root directory to current tab
            var tab = _appSettings.RootDirectoryTabs.FirstOrDefault(t => t.Id == tabId);
            if (tab == null)
            {
                return RedirectToAction("DirectoryNotFound", "Error", new { requestedDirectoryPath = path });
            }

            path = path.TrimStart('/');
            var absolutePath = Path.Combine(tab.AbsolutePath, path);

            if (System.IO.File.Exists(absolutePath))
            {
                var stream = System.IO.File.OpenRead(absolutePath);
                return File(stream, "octlet/stream", absolutePath.Split('\\').Last());
            }

            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
