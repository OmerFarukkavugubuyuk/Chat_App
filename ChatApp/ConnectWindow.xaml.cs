using System.Windows;

namespace ChatApp;

public partial class ConnectWindow : Window
{
    public string Nick { get; private set; } = "";
    public string Host { get; private set; } = "";
    public int Port { get; private set; }
    public bool IsServer { get; private set; }

    public ConnectWindow()
    {
        InitializeComponent();
    }

    private void ServerButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = "";

        if (string.IsNullOrWhiteSpace(NickBox.Text))
        { ErrorText.Text = "Kullanıcı adı boş bırakılamaz."; return; }

        if (!int.TryParse(PortBox.Text, out int port) || port < 1 || port > 65535)
        { ErrorText.Text = "Geçerli bir port girin (1-65535)."; return; }

        Nick = NickBox.Text.Trim();
        Port = port;
        IsServer = true;
        DialogResult = true;
    }

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = "";
        IpPanel.Visibility = Visibility.Visible;

        if (string.IsNullOrWhiteSpace(NickBox.Text))
        { ErrorText.Text = "Kullanıcı adı boş bırakılamaz."; return; }

        if (string.IsNullOrWhiteSpace(IpBox.Text))
        { ErrorText.Text = "Sunucu IP adresi boş bırakılamaz."; return; }

        if (!int.TryParse(PortBox.Text, out int port) || port < 1 || port > 65535)
        { ErrorText.Text = "Geçerli bir port girin (1-65535)."; return; }

        Nick = NickBox.Text.Trim();
        Host = IpBox.Text.Trim();
        Port = port;
        IsServer = false;
        DialogResult = true;
    }
}
