using Microsoft.AspNetCore.Mvc;

namespace TKC.Controllers
{
    [Route("[controller]")]
    public class FileController : Controller
    {
        [HttpGet("Documents/{fileName}")]
        public IActionResult GetDocumentFile(string fileName)
        {

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            string contentType = GetContentType(fileName);
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            FileStreamResult fileResult;
            if (contentType == "application/octet-stream")
            {
                fileResult = new FileStreamResult(fileStream, contentType)
                {
                    FileDownloadName = fileName
                };
            }
            else
            {
                fileResult = new FileStreamResult(fileStream, contentType);
            }

            return fileResult;
        }


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

        [HttpHead("{fileName}")]
        public ActionResult GetFileInfo(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            long fileSize = new FileInfo(filePath).Length;

            string contentType = GetContentType(fileName);

            Response.Headers.Add("Accept-Ranges", "bytes");
            Response.Headers.Add("Content-Type", contentType);
            Response.Headers.Add("Content-Length", $"{fileSize}");

            return Ok();
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
                string contentType = GetContentType(fileName);

                // Open the file stream
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                long fileSize = fileStream.Length;

                // Check if the request includes a Range header
                var rangeHeader = Request.Headers["Range"].ToString();
                if (!string.IsNullOrEmpty(rangeHeader))
                {
                    // Parse the range header
                    var rangeValue = Microsoft.Net.Http.Headers.RangeHeaderValue.Parse(rangeHeader);
                    var range = rangeValue.Ranges.FirstOrDefault();

                    // Extract start position, defaulting to 0 if not specified
                    var start = range.From ?? 0;
                    // Extract end position, defaulting to the end of the file if not specified
                    var end = range.To ?? fileSize - 1;

                    // Adjust start and end if necessary
                    start = Math.Max(0, start);
                    end = Math.Min(fileSize - 1, end);

                    // Calculate the length of the partial content
                    var length = end - start + 1;

                    // Set the appropriate headers for partial content
                    Response.Headers.Add("Accept-Ranges", "bytes");
                    Response.Headers.Add("Content-Type", contentType);
                    Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileStream.Length}");
                    Response.Headers.Add("Content-Length", $"{length}");
                    Response.StatusCode = 206;

                    // Return the partial content
                    fileStream.Seek(start, SeekOrigin.Begin);
                    return File(fileStream, contentType);
                }
                else
                {
                    Response.Headers.Add("Accept-Ranges", "bytes");

                    // Return the full content
                    return File(fileStream, contentType);
                }
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        //[HttpGet("{fileName}")]
        public ActionResult GetFileV1(string fileName)
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
                string contentType = GetContentType(fileName);

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

        private string GetContentType(string fileName)
        {
            string contentType = "audio/mpeg";
            if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "audio/mpeg";
            }
            else if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "application/pdf";
            }
            else if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "audio/wav";
            }
            else if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/jpeg";
            }
            else
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}

