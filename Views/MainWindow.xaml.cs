﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultiImageDownloader.ViewModels;
using System.Windows;

namespace MultiImageDownloader.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}