using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MultiImageDownloader.Services
{
    public class ImageDownloadService
    {
        private readonly HttpClient _httpClient;

        public ImageDownloadService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<BitmapImage> DownloadImageAsync(
            string url,
            IProgress<double> progress,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var receivedBytes = 0L;
                var buffer = new byte[8192];

                await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                await using var memoryStream = new MemoryStream();

                while (true)
                {
                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead == 0) break;

                    await memoryStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    receivedBytes += bytesRead;

                    if (totalBytes > 0)
                        progress?.Report((double)receivedBytes / totalBytes * 100);
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки: {ex.Message}");
            }
        }
    }
}
