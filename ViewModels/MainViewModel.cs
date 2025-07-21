using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiImageDownloader.Models;
using MultiImageDownloader.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MultiImageDownloader.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ImageDownloadService _downloadService;
        private CancellationTokenSource[] _cancellationTokenSources;

        public ObservableCollection<ImageDownloadItem> DownloadItems { get; }

        [ObservableProperty]
        private double _totalProgress;

        public MainViewModel()
        {
            _downloadService = new ImageDownloadService();
            _cancellationTokenSources = new CancellationTokenSource[3];
            DownloadItems = new ObservableCollection<ImageDownloadItem>
            {
                new ImageDownloadItem(),
                new ImageDownloadItem(),
                new ImageDownloadItem()
            };
        }

        [RelayCommand]
        private async Task StartDownload(int index)
        {
            // Проверяем валидность индекса
            if (index < 0 || index >= DownloadItems.Count)
            {
                Debug.WriteLine($"Некорректный индекс: {index}");
                return;
            }

            var item = DownloadItems[index];
            if (string.IsNullOrWhiteSpace(item.Url))
                return;

            // Отменяем текущую загрузку (если есть)
            _cancellationTokenSources[index]?.Cancel();
            _cancellationTokenSources[index] = new CancellationTokenSource();

            item.IsDownloading = true;
            var progress = new Progress<double>(value => item.Progress = value);

            try
            {
                item.Image = await _downloadService.DownloadImageAsync(
                    item.Url,
                    progress,
                    _cancellationTokenSources[index].Token);
            }
            catch (OperationCanceledException)
            {
                item.Progress = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
            }
            finally
            {
                item.IsDownloading = false;
                UpdateTotalProgress();
            }
        }

        [RelayCommand]
        private void StopDownload(int index)
        {
            if (index < 0 || index >= _cancellationTokenSources.Length)
            {
                Debug.WriteLine($"Некорректный индекс для отмены: {index}");
                return;
            }
            _cancellationTokenSources[index]?.Cancel();
        }

        [RelayCommand]
        private async Task DownloadAll()
        {
            for (int i = 0; i < DownloadItems.Count; i++)
                await StartDownload(i); // Используем сгенерированный метод
        }

        private void UpdateTotalProgress()
        {
            double sum = 0;
            int activeDownloads = 0;

            foreach (var item in DownloadItems)
            {
                if (item.IsDownloading)
                {
                    sum += item.Progress;
                    activeDownloads++;
                }
            }

            TotalProgress = activeDownloads > 0 ? sum / activeDownloads : 0;
        }
    }
}