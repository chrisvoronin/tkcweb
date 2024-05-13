namespace TKC.Models
{
    public class MP3Parser
    {
        public static double CalculateDurationInSeconds(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
            using (var file = TagLib.File.Create(filePath))
            {
                var duration = file.Properties.Duration.TotalSeconds;
                return duration;
            }
        }
    }
}

/*
 private async Task ProcessAllSermons()
    {
        var all = await _context.Sermons.ToListAsync();
        int count = all.Count;
        int index = 0;
        foreach (var item in all)
        {
            if (item.Id >= 545 && !string.IsNullOrWhiteSpace(item.AudioUrl))
            {
                Console.WriteLine($"Processing {index} of {count}");
                await ProcessFile(item);
            }
            index += 1;
        }
    }

    private async Task ProcessFile(Sermon item)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", item.AudioUrl!);
                // Calculate file size
                FileInfo fileInfo = new FileInfo(filePath);

                // Get the file size in bytes
                long fileSize = fileInfo.Length;
                // Calculate duration
                double durationInSeconds = MP3Parser.CalculateDurationInSeconds(filePath);

                // Clean up temporary file
                item.AudioDuration = durationInSeconds;
                item.AudioFileSize = fileSize;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing {item.AudioUrl!}: {ex.Message}");
            }
        }
    }

    private async Task Process(Sermon item)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(item.AudioUrl!);
                if (response.IsSuccessStatusCode)
                {
                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    {
                        // Calculate file size
                        long fileSize = stream.Length;

                        // Save the stream to a temporary file
                        string tempFilePath = Path.GetTempFileName() + ".mp3";
                        using (FileStream fileStream = System.IO.File.Create(tempFilePath))
                        {
                            await stream.CopyToAsync(fileStream);
                        }

                        // Calculate duration
                        double durationInSeconds = MP3Parser.CalculateDurationInSeconds(tempFilePath);

                        // Clean up temporary file
                        System.IO.File.Delete(tempFilePath);
                        item.AudioDuration = durationInSeconds;
                        item.AudioFileSize = fileSize;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to download {item.AudioUrl!}. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing {item.AudioUrl!}: {ex.Message}");
            }
        }
    }
 
 
 */