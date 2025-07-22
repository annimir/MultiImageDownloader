using Microsoft.Extensions.Logging;
using MultiImageDownloader.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MultiImageDownloader.Services
{
    public class ImageDownloader : IImageDownloader
    {
        private readonly ILogger<ImageDownloader> _logger;

        public ImageDownloader(ILogger<ImageDownloader> logger)
        {
            _logger = logger;
        }

        public async Task<ImageSource> DownloadImageAsync(string url, IProgress<double> progress, CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var receivedBytes = 0L;
                var buffer = new byte[8192];

                await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var memoryStream = new MemoryStream();

                while (true)
                {
                    var bytesRead = await contentStream.ReadAsync(buffer, cancellationToken);
                    if (bytesRead == 0) break;

                    await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                    receivedBytes += bytesRead;

                    if (totalBytes > 0)
                    {
                        var currentProgress = (double)receivedBytes / totalBytes * 100;
                        progress?.Report(currentProgress);
                    }
                }

                memoryStream.Position = 0;
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memoryStream;
                bitmap.EndInit();
                bitmap.Freeze();

                return bitmap;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading image from {Url}", url);
                return null;
            }
        }
    }
}
