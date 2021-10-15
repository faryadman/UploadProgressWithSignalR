using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using UploadProgressWithSignalR.Core;

namespace UploadProgressWithSignalR.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHubContext<NotifyHub> _notifyHub;



        public IndexModel(ILogger<IndexModel> logger, IHubContext<NotifyHub> notifyHub)
        {
            _logger = logger;
            _notifyHub = notifyHub;
        }

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPost(IFormFile file, [FromServices] IWebHostEnvironment env)
        {
            try
            {
                byte[] buffer = new byte[16 * 1024];
                long totalByte = file.Length;
                using FileStream output = System.IO.File.Create(env.ContentRootPath + "/files/" + file.FileName);
                using Stream input = file.OpenReadStream();
                long totalReadBytes = 0;
                int readBytes;
                while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await output.WriteAsync(buffer, 0, readBytes);
                    totalReadBytes += readBytes;
                    int progress = (int)((float)totalReadBytes / (float)totalByte * 100.0);
                    await _notifyHub.Clients.All.SendAsync("updateProgress", progress);
                    await Task.Delay(200);//It is only to make the progress slower!
                }
                TempData["message"] = $"{file.FileName} Uploaded!";
                return Page();
            }
            catch (Exception ex)
            {
                TempData["exception"] = $"{file.FileName} Filed!\n Because {ex.Message}";
                _logger.LogError(ex.Message);
                return Page();
            }
        }
    }
}