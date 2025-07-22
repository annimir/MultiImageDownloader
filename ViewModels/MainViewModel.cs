using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiImageDownloader.Models;
using Microsoft.Extensions.Logging;
using MultiImageDownloader.Services;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.IO;


namespace MultiImageDownloader.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IImageDownloader _imageDownloader;
        private readonly ILogger<MainViewModel> _logger;

        public ObservableCollection<DownloadItem> DownloadItems { get; } = new();

        [ObservableProperty]
        private double _totalProgress;

        [ObservableProperty]
        private string _statusText;

        public MainViewModel(IImageDownloader imageDownloader, ILogger<MainViewModel> logger)
        {
            _imageDownloader = imageDownloader;
            _logger = logger;

            // Initialize 3 download slots
            for (int i = 0; i < 3; i++)
            {
                DownloadItems.Add(new DownloadItem());
            }
        }

        [RelayCommand]
        private async Task StartDownloadAsync(DownloadItem item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Url)) return;
            if (item.IsDownloading) return;

            item.IsDownloading = true;
            item.CancellationTokenSource = new CancellationTokenSource();

            var progress = new Progress<double>(p =>
            {
                item.Progress = p;
                UpdateProgress();
            });

            try
            {
                _logger.LogInformation("Starting download from {Url}", item.Url);
                StatusText = $"Downloading: {GetFileNameFromUrl(item.Url)}";

                item.Image = await _imageDownloader.DownloadImageAsync(
                    item.Url,
                    progress,
                    item.CancellationTokenSource.Token);

                StatusText = $"Completed: {GetFileNameFromUrl(item.Url)}";
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Download canceled for {Url}", item.Url);
                StatusText = $"Canceled: {GetFileNameFromUrl(item.Url)}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading from {Url}", item.Url);
                StatusText = $"Error: {GetFileNameFromUrl(item.Url)}";
            }
            finally
            {
                item.IsDownloading = false;
                UpdateProgress();
            }
        }

        [RelayCommand]
        private void StopDownload(DownloadItem item)
        {
            if (item == null || !item.IsDownloading) return;

            _logger.LogInformation("Stopping download from {Url}", item.Url);
            item.CancellationTokenSource?.Cancel();
        }

        [RelayCommand]
        private async Task DownloadAllAsync()
        {
            _logger.LogInformation("Starting all downloads");
            StatusText = "Downloading all images...";

            var tasks = DownloadItems
                .Where(item => !string.IsNullOrWhiteSpace(item.Url))
                .Select(item => StartDownloadAsync(item));

            await Task.WhenAll(tasks);
            StatusText = "All downloads completed";
        }

        private void UpdateProgress()
        {
            var activeDownloads = DownloadItems.Where(x => x.IsDownloading).ToList();
            TotalProgress = activeDownloads.Count > 0 ? activeDownloads.Average(x => x.Progress) : 0;
        }

        private static string GetFileNameFromUrl(string url)
        {
            try
            {
                return Path.GetFileName(new Uri(url).LocalPath);
            }
            catch
            {
                return url.Length > 30 ? url[..30] + "..." : url;
            }
        }
    }
}