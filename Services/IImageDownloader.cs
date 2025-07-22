using System.Windows.Media;

namespace MultiImageDownloader.Models
{
    public interface IImageDownloader
    {
        Task<ImageSource> DownloadImageAsync(string url, IProgress<double> progress, CancellationToken cancellationToken);
    }
}
