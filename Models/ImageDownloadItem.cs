using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MultiImageDownloader.Models
{
    public partial class ImageDownloadItem : ObservableObject
    {
        [ObservableProperty]
        private string _url;

        [ObservableProperty]
        private BitmapImage _image;

        [ObservableProperty]
        private double _progress;

        [ObservableProperty]
        private bool _isDownloading;
    }
}
