using System;
using System.Net.Sockets;
using System.Windows.Forms;
using ClientChatLibrary;


namespace GameChatUI
{
    public partial class Form1 : Form
    {
        private ClientChat _chatClient = new ClientChat();

        public Form1()
        {
            InitializeComponent();
            _chatClient.OnMessageReceived += OnMessageReceived;
        }

   
        private void OnMessageReceived(string message)
        {
            //if (this.InvokeRequired)
            //{
                this.Invoke((MethodInvoker)delegate
                {
                    chatHistory.AppendText(message + Environment.NewLine);
                });
            //}
            //else
            //{
            //    chatHistory.AppendText(message + Environment.NewLine);
            //}
        }

        private async void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_chatClient.IsConnected)
                {
                    chatHistory.AppendText("[System]: You are already connected." + Environment.NewLine);
                }
                else
                {
                    await _chatClient.ConnectAsync("127.0.0.1");
                    chatHistory.AppendText("[System]: Connected to the server successfully." + Environment.NewLine);
                }
            }
            catch (SocketException)
            {
                chatHistory.AppendText("[Error]: Could not connect. Is your Ncat server running on port 13000?" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                chatHistory.AppendText($"[Error]: An unexpected connection error occurred: {ex.Message}" + Environment.NewLine);
            }
        }

        private async void SendBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(messageBox.Text)) return;

            try
            {
                string textToSend = messageBox.Text;

                await _chatClient.SendMessageAsync(textToSend);

                chatHistory.AppendText($"[You]: {textToSend}" + Environment.NewLine);
                messageBox.Clear();
            }
            catch (Exception ex)
            {
                chatHistory.AppendText($"[Error]: Communication failed. {ex.Message}" + Environment.NewLine);
            }
        }

        private void DisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_chatClient.IsConnected)
            {
                _chatClient.Disconnect();
                chatHistory.AppendText("[System]: Disconnected from server." + Environment.NewLine);
            }
            else
            {
                chatHistory.AppendText("[System]: You are not currently connected." + Environment.NewLine);
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _chatClient.Disconnect();
            Application.Exit();
        }

    }
}
