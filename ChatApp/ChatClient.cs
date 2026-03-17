using System.Net.Sockets;
using System.Text;

namespace ChatApp;

public class ChatClient
{
    private TcpClient? _client;
    private NetworkStream? _stream;

    public event Action<string>? MessageReceived;
    public event Action? Disconnected;

    public async Task ConnectAsync(string host, int port)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(host, port);
        _stream = _client.GetStream();
        _ = ReceiveLoopAsync();
    }

    public async Task SendAsync(string message)
    {
        if (_stream == null) return;
        var data = Encoding.UTF8.GetBytes(message);
        await _stream.WriteAsync(data);
    }

    public void Disconnect()
    {
        _stream?.Close();
        _client?.Close();
    }

    private async Task ReceiveLoopAsync()
    {
        var buffer = new byte[4096];
        try
        {
            while (true)
            {
                int n = await _stream!.ReadAsync(buffer);
                if (n == 0) break;
                var msg = Encoding.UTF8.GetString(buffer, 0, n);
                MessageReceived?.Invoke(msg);
            }
        }
        catch { }
        finally
        {
            Disconnected?.Invoke();
        }
    }
}
