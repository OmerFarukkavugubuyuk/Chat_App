using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatApp;

public class ChatServer
{
    private TcpListener _listener;
    private readonly List<TcpClient> _clients = new();
    private readonly object _lock = new();
    private bool _running;

    public event Action<string>? MessageReceived;

    public void Start(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        _running = true;
        Task.Run(AcceptClients);
    }

    public void Stop()
    {
        _running = false;
        _listener.Stop();
        lock (_lock)
        {
            foreach (var c in _clients) c.Close();
            _clients.Clear();
        }
    }

    private async Task AcceptClients()
    {
        while (_running)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync();
                lock (_lock) _clients.Add(client);
                _ = HandleClient(client);
            }
            catch { break; }
        }
    }

    private async Task HandleClient(TcpClient client)
    {
        var stream = client.GetStream();
        var buffer = new byte[4096];
        try
        {
            while (true)
            {
                int n = await stream.ReadAsync(buffer);
                if (n == 0) break;
                var msg = Encoding.UTF8.GetString(buffer, 0, n);
                MessageReceived?.Invoke(msg);
                await BroadcastAsync(msg, client);
            }
        }
        catch { }
        finally
        {
            lock (_lock) _clients.Remove(client);
            client.Close();
        }
    }

    private async Task BroadcastAsync(string message, TcpClient? except = null)
    {
        var data = Encoding.UTF8.GetBytes(message);
        List<TcpClient> snapshot;
        lock (_lock) snapshot = new List<TcpClient>(_clients);
        foreach (var c in snapshot)
        {
            if (c == except) continue;
            try { await c.GetStream().WriteAsync(data); }
            catch { }
        }
    }

    public async Task SendAsServerAsync(string message)
    {
        await BroadcastAsync(message);
    }
}
