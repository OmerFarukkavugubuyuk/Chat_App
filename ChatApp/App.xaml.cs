using System.Windows;

namespace ChatApp;

public partial class App : Application
{
    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        var connectWindow = new ConnectWindow();
        if (connectWindow.ShowDialog() != true)
        {
            Shutdown();
            return;
        }

        if (connectWindow.IsServer)
        {
            try
            {
                var server = new ChatServer();
                server.Start(connectWindow.Port);
                var window = new ChatWindow(server, connectWindow.Nick, connectWindow.Port);
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sunucu başlatılamadı:\n{ex.Message}",
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
        else
        {
            var client = new ChatClient();
            try
            {
                await client.ConnectAsync(connectWindow.Host, connectWindow.Port);
                var window = new ChatWindow(client, connectWindow.Nick, connectWindow.Host, connectWindow.Port);
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sunucuya bağlanılamadı.\n{connectWindow.Host}:{connectWindow.Port}\n\n{ex.Message}",
                    "Bağlantı Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
