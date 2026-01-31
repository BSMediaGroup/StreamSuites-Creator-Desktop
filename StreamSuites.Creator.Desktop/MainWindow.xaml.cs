using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace StreamSuites.Creator.Desktop;

public partial class MainWindow : Window
{
    private static readonly Uri CreatorUri = new("https://creator.streamsuites.app");
    private static readonly string[] AllowedHosts =
    [
        "creator.streamsuites.app",
        "auth.streamsuites.app"
    ];

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
        ConfigureWebView();
        CreatorWebView.Source = CreatorUri;
    }

    private void ConfigureWebView()
    {
        CreatorWebView.CoreWebView2.NavigationStarting += OnNavigationStarting;
        CreatorWebView.CoreWebView2.NewWindowRequested += OnNewWindowRequested;

#if DEBUG
        CreatorWebView.CoreWebView2.Settings.AreDevToolsEnabled = true;
#else
        CreatorWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
#endif

        string currentUserAgent = CreatorWebView.CoreWebView2.Settings.UserAgent ?? string.Empty;
        string appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
        string brandedUserAgent = string.IsNullOrWhiteSpace(currentUserAgent)
            ? $"StreamSuitesCreatorDesktop/{appVersion}"
            : $"{currentUserAgent} StreamSuitesCreatorDesktop/{appVersion}";
        CreatorWebView.CoreWebView2.Settings.UserAgent = brandedUserAgent;
    }

    private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (!TryGetAllowedUri(e.Uri, out Uri? uri))
        {
            e.Cancel = true;
            OpenExternal(uri ?? e.Uri);
        }
    }

    private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        if (TryGetAllowedUri(e.Uri, out Uri? uri))
        {
            e.Handled = true;
            CreatorWebView.CoreWebView2.Navigate(uri!.ToString());
            return;
        }

        e.Handled = true;
        OpenExternal(uri ?? e.Uri);
    }

    private static bool TryGetAllowedUri(string? rawUri, out Uri? uri)
    {
        uri = null;
        if (!Uri.TryCreate(rawUri, UriKind.Absolute, out Uri? parsed))
        {
            return false;
        }

        uri = parsed;

        if (!string.Equals(parsed.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        foreach (string host in AllowedHosts)
        {
            if (string.Equals(parsed.Host, host, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static void OpenExternal(string? rawUri)
    {
        if (!Uri.TryCreate(rawUri, UriKind.Absolute, out Uri? uri))
        {
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = uri.ToString(),
            UseShellExecute = true
        });
    }
}
