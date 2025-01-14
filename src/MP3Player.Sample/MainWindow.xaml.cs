﻿using System.ComponentModel;
using System.Windows;

namespace MP3Player.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SimpleMp3PlayerViewModel _simpleViewModel;
        private readonly StreamingViewModel _streamingViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _simpleViewModel = new SimpleMp3PlayerViewModel();
            _streamingViewModel = new StreamingViewModel();
            DataContext = _simpleViewModel;
            Closing += OnClosing;
        }

        private void OnSimple(object sender, RoutedEventArgs e)
        {
            DataContext = _simpleViewModel;
        }

        private void OnStreaming(object sender, RoutedEventArgs e)
        {
            DataContext = _streamingViewModel;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _simpleViewModel?.Dispose();
            _streamingViewModel?.Dispose();
        }
    }
}
