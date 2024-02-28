using System;
namespace TKC.Controllers
{
	public static class FileUtility
	{
        public static async Task<string> SaveFile(IFormFile file)
        {
            return await SaveFile(file, "Uploads");
        }

        public static async Task<string> SaveStaffPhoto(IFormFile file)
        {
            return await SaveFile(file, "Staff");
        }

        private static async Task<string> SaveFile(IFormFile file, string DIRECTORY)
        {
            // build file name by default with just its existing name
            var fileName = file.FileName;

            //replace empty spaces
            fileName = fileName.Replace(" ", "_");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), DIRECTORY, fileName);

            // if it exists, lets add a guid to not override, otherwise use filename as is.
            if (System.IO.File.Exists(filePath))
            {
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                filePath = Path.Combine(Directory.GetCurrentDirectory(), DIRECTORY, fileName);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }
    }
}

