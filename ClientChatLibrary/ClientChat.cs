using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

                                                // Many different resources have been used to understand the concepts of this assignment,
                                                // I can provide more details if needed.
namespace ClientChatLibrary
{
    public class ClientChat
    {
        private TcpClient _client;             // reference (naming convention) https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names                                 
        private NetworkStream _stream;
        private const int Port = 13000;
        private readonly SemaphoreSlim _networkLock = new SemaphoreSlim(1, 1);   // reference: https://www.youtube.com/shorts/mr8kdAauc7E
        private CancellationTokenSource _cts;            
        public delegate void MessageReceivedHandler(string message);             
        public event MessageReceivedHandler OnMessageReceived;    
        public bool IsConnected => _client != null && _client.Connected;


        public async Task ConnectAsync(string server)      // good reference for async/await and more: https://www.youtube.com/watch?v=eYHOZhMA7OI
        {
            if (IsConnected) return;

            try                                              
            {
                _client = new TcpClient();
                await _client.ConnectAsync(server, Port);
                _stream = _client.GetStream();

                
                _cts = new CancellationTokenSource();   


                _ = ListenForIncomingMassages(_cts.Token);
            }
            catch (Exception)
            {
                throw;
            }
        }

       
        private async Task ListenForIncomingMassages(CancellationToken token)   // a good reference (about CancellationToken): https://learn.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads
        {
            while (IsConnected && !token.IsCancellationRequested)
            {
                try
                {
                    string response = await GetStringMessageAsync();

                    if (!string.IsNullOrEmpty(response))
                    {
                        
                        OnMessageReceived?.Invoke(response);
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to the server.");

            await _networkLock.WaitAsync();
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(message + Environment.NewLine);
                await _stream.WriteAsync(data, 0, data.Length);
                await _stream.FlushAsync();
            }
            catch (SocketException)
            {
                Disconnect();
                throw;
            }
            finally
            {
                _networkLock.Release();
            }
        }

        private async Task<string> GetStringMessageAsync()
        {
            if (!IsConnected) return string.Empty;

            byte[] data = new byte[256];
            int bytes = await _stream.ReadAsync(data, 0, data.Length);

            if (bytes == 0)
            {
                Disconnect();
                return "[System]: Server has closed the connection.";
            }

            return Encoding.ASCII.GetString(data, 0, bytes);
        }

        public void Disconnect()
        {
            _cts?.Cancel();
            _stream?.Close();
            _client?.Close();
            _stream = null;
            _client = null;
        }
    }
}
