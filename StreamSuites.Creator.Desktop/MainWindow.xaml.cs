using System;
using System.IO;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace StreamSuites.Creator.Desktop;

public partial class MainWindow : Window
{
    private static readonly Uri CreatorUri = new("https://creator.streamsuites.app");

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        string userDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StreamSuites",
            "CreatorDesktop",
            "UserData");

        CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
        await CreatorWebView.EnsureCoreWebView2Async(environment);
        CreatorWebView.Source = CreatorUri;
    }
}
