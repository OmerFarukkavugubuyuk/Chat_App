using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ChatApp;

public partial class ChatWindow : Window
{
    private readonly ChatClient? _client;
    private readonly ChatServer? _server;
    private readonly string _nick;
    private readonly ObservableCollection<string> _messages = new();

    public ChatWindow(ChatServer server, string nick, int port)
    {
        InitializeComponent();
        _server = server;
        _nick = nick;
        MessageList.ItemsSource = _messages;
        StatusText.Text = $"Sunucu | {nick} | Port: {port}";

        _server.MessageReceived += OnMessageReceived;
        AddMessage("[Sistem] Sunucu başlatıldı. Arkadaşlarınız bağlanabilir.");
    }

    public ChatWindow(ChatClient client, string nick, string host, int port)
    {
        InitializeComponent();
        _client = client;
        _nick = nick;
        MessageList.ItemsSource = _messages;
        StatusText.Text = $"İstemci | {nick} | {host}:{port}";

        _client.MessageReceived += OnMessageReceived;
        _client.Disconnected += () => Dispatcher.Invoke(() =>
            AddMessage("[Sistem] Sunucuyla bağlantı kesildi."));
    }

    private void OnMessageReceived(string msg)
    {
        Dispatcher.Invoke(() => AddMessage(msg));
    }

    private void AddMessage(string msg)
    {
        _messages.Add(msg);
        Scroller.ScrollToBottom();
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e) => await SendAsync();

    private async void InputBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) await SendAsync();
    }

    private async Task SendAsync()
    {
        var text = InputBox.Text.Trim();
        if (string.IsNullOrEmpty(text)) return;
        InputBox.Clear();

        var msg = $"[{_nick}] {text}";
        AddMessage(msg);

        if (_server != null)
            await _server.SendAsServerAsync(msg);
        else if (_client != null)
            await _client.SendAsync(msg);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _server?.Stop();
        _client?.Disconnect();
        Application.Current.Shutdown();
    }
}
