using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using MultiImageDownloader.Models;
using MultiImageDownloader.Services;
using MultiImageDownloader.ViewModels;
using MultiImageDownloader.Views;
using System.Windows;

namespace MultiImageDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure =>
            {
                configure.AddDebug();
                configure.AddConsole();
            });

            services.AddHttpClient();

            services.AddSingleton<IImageDownloader, Services.ImageDownloader>();
            services.AddSingleton<ViewModels.MainViewModel>();
            services.AddSingleton<MainWindow>();
        }
    }
}