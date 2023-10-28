using Microsoft.AspNetCore.Mvc;

namespace TKC.Controllers
{
    [Route("[controller]")]
    public class FileController : Controller
    {
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

                // Open the file stream
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                string contentType = "audio/mpeg";
                if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "audio/mpeg";
                }
                else if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "application/pdf";
                }
                var fileResult = new FileStreamResult(fileStream, contentType);
                return fileResult;

            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
            
        }
        /*
        // Route: /music/{id}/mp3
        [HttpGet("{id}/mp3")]
        public IActionResult MP3(int id)
        {
            // Generate or fetch the PDF file based on the 'id'
            byte[] pdfBytes = GeneratePdf(id);

            // Specify the content type for PDF
            const string contentType = "application/pdf";

            // Return the PDF file as a download
            return File(pdfBytes, contentType, $"music_{id}.pdf");
        }

        // Route: /music/{id}/pdf
        [HttpGet("{id}/pdf")]
        public IActionResult PDF(int id)
        {
            var m = GetMusicById(id);
            // Generate or fetch the PDF file based on the 'id'
            byte[] pdfBytes = GeneratePdf(id);

            // Specify the content type for PDF
            const string contentType = "application/pdf";

            // Return the PDF file as a download
            return File(pdfBytes, contentType, $"music_{id}.pdf");
        }


        // Dummy method to generate PDF bytes (replace this with your logic)
        private byte[] GeneratePdf(int id)
        {
            // Replace this with your logic to generate or fetch the PDF
            // For simplicity, using a dummy content here
            string dummyContent = $"PDF content for music {id}";
            return System.Text.Encoding.UTF8.GetBytes(dummyContent);
        }
        */
    }
}

