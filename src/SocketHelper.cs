using System;
using System.Net.Sockets;
using System.Text;

namespace Qingame.Mail
{
    /// <summary>
    /// socket助手
    /// </summary>
    public class SocketHelper
    {
        private int _state = -1;
        private string _host = "";
        private int _port = 25;
        private Socket _socket;

        public SocketHelper(string host, int port)
        {
            _host = host;
            _port = port;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_host, _port);
        }
        public int Timeout
        {
            get
            {
                return _socket.SendTimeout;
            }
            set
            {
                _socket.SendTimeout = value;
                _socket.ReceiveTimeout = value;
            }
        }
        public int GetState()
        {
            return _state;
        }
        public void SendData(byte[] bytes)
        {
            if (_state != 221)
            {
                _socket.Send(bytes, SocketFlags.None);
            }
        }
        public void SendCommand(string cmd)
        {
            if (_state != 221)
            {
                _socket.Send(Encoding.UTF8.GetBytes(cmd + "\r\n"), SocketFlags.None);
                //
                byte[] bytes = new byte[1024];
                int count = _socket.Receive(bytes);
                string recvData = Encoding.UTF8.GetString(bytes, 0, count);
                _state = -1;
                if (string.IsNullOrEmpty(recvData) == false)
                {
                    string[] array = recvData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if(array != null && array.Length > 0)
                    {
                        _state = ToResponseState(array[array.Length - 1]);
                    }
                }
            }
        }
        private int ToResponseState(string line)
        {
            if (string.IsNullOrEmpty(line) == false && line.Length >= 3 && IsNumber(line[0]) && IsNumber(line[1]) && IsNumber(line[2]))
            {
                return Convert.ToInt32(line.Substring(0, 3));
            }
            return -1;
        }

        private bool IsNumber(char c)
        {
            return c >= '0' && c <= '9';
        }

        public void Close()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        public void ReConnect()
        {
            Connect(_host, _port);
        }

        public void Connect(string host, int port)
        {
            Close();
            _socket.Connect(host, port);
            _host = host;
            _port = port;
        }
    }
}