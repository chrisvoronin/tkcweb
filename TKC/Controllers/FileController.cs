using Microsoft.AspNetCore.Mvc;

namespace TKC.Controllers
{
    [Route("[controller]")]
    public class FileController : Controller
    {
        [HttpGet("Staff/{fileName}")]
        public IActionResult GetStaffFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Staff", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileContents = System.IO.File.ReadAllBytes(filePath);
            return File(fileContents, "image/jpeg");
        }

        [HttpGet("{fileName}")]
        public ActionResult GetFile(string fileName)
        {
            try
            {
                // Get the physical path to the file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                // Determine the content type based on file extension
                string contentType;
                if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "audio/mpeg";
                }
                else if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "application/pdf";
                }
                else
                {
                    // Handle other file types if needed
                    return BadRequest("Unsupported file type");
                }

                // Open the file stream
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var fileResult = new FileStreamResult(fileStream, contentType)
                {
                    FileDownloadName = fileName
                };

                // if pdf, prompt download instead of redirect
                if (contentType == "application/pdf")
                {
                    Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);
                }

                return fileResult;
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}

