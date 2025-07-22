using CommunityToolkit.Mvvm.ComponentModel;
using MultiImageDownloader.Models;
using System.Windows.Media;

namespace MultiImageDownloader.Models;

public class DownloadItem : ObservableObject
{
    private string _url;
    private ImageSource _image;
    private bool _isDownloading;
    private double _progress;
    private CancellationTokenSource _cancellationTokenSource;

    public string Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }

    public ImageSource Image
    {
        get => _image;
        set => SetProperty(ref _image, value);
    }

    public bool IsDownloading
    {
        get => _isDownloading;
        set => SetProperty(ref _isDownloading, value);
    }

    public double Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    public CancellationTokenSource CancellationTokenSource
    {
        get => _cancellationTokenSource;
        set => SetProperty(ref _cancellationTokenSource, value);
    }
}