using System;
using System.Text;
using System.Threading;

namespace Qingame.Mail
{
    /// <summary>
    /// </summary>
    public class SmtpServer
    {
        private string _host;
        private int _port = 25;
        private bool _auth = false;
        private string _user;
        private string _pw;
        private SocketHelper _helper = null;
        private TalkState _state = TalkState.WaitInit;
        private int _timeout;

        public enum TalkState
        {
            WaitInit, Initialized, SignedIn, LoginFailed, Ended
        }

        private SocketHelper GetHelper()
        {
            if (_helper == null)
            {
                _helper = new SocketHelper(_host, _port);
                _helper.Timeout = _timeout;
            }
            return _helper;
        }
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
                if (_helper != null)
                {
                    _helper.Timeout = _timeout;
                }
            }
        }
        public string host
        {
            get
            {
                return _host;
            }
        }

        public int port
        {
            get
            {
                return _port;
            }
        }

        public bool RequireAuthorization
        {
            get
            {
                return _auth;
            }
            set
            {
                _auth = value;
            }
        }

        public SmtpServer(string host, int port = 25)
        {
            _host = host;
            _port = port;
        }

        public void Init()
        {
            GetHelper().SendCommand("EHLO " + _host);
            _state = TalkState.Initialized;
        }
        public void SetCredentials(string user,string pw)
        {
            _user = user;
            _pw = pw;
        }
        public bool Login()
        {
            if (this.RequireAuthorization)
            {
                GetHelper().SendCommand("AUTH LOGIN");
                GetHelper().SendCommand(Convert.ToBase64String(Encoding.UTF8.GetBytes(_user)));
                GetHelper().SendCommand(Convert.ToBase64String(Encoding.UTF8.GetBytes(_pw)));
                if (GetHelper().GetState() == 235)
                {
                    _state = TalkState.SignedIn;
                    return true;
                }
                else
                {
                    _state = TalkState.LoginFailed;
                    return false;
                }
            }
            else
            {
                _state = TalkState.SignedIn;
                return true;
            }
        }

        public void SendKeep(MailMessage msg)
        {
            if ((_state == TalkState.Initialized && !this.RequireAuthorization) || _state == TalkState.SignedIn)
            {
                GetHelper().SendCommand("MAIL From:" + msg.From);
                ParseAddTo(msg.To);
                GetHelper().SendCommand("DATA");
                GetHelper().SendData(Encoding.Default.GetBytes(msg.ToString()));
                GetHelper().SendCommand(".");
            }
        }

        public void Quit()
        {
            GetHelper().SendCommand("QUIT");
            GetHelper().Close();
            _state = TalkState.Ended;
        }

        public bool Send(MailMessage msg)
        {
            if (_state == TalkState.Ended)
            {
                return false;
            }
            GetHelper().SendCommand("EHLO " + _host);
            if (this.RequireAuthorization)
            {
                GetHelper().SendCommand("AUTH LOGIN");
                GetHelper().SendCommand(Convert.ToBase64String(Encoding.UTF8.GetBytes(_user)));
                GetHelper().SendCommand(Convert.ToBase64String(Encoding.UTF8.GetBytes(_pw)));
                GetHelper().SendCommand("MAIL From:" + msg.From);
                if (GetHelper().GetState() != 235)
                {
                    Quit();
                    return false;
                }
            }
            ParseAddTo(msg.To);
            GetHelper().SendCommand("DATA");
            GetHelper().SendData(Encoding.Default.GetBytes(msg.ToString()));
            GetHelper().SendCommand(".");
            Quit();
            return true;
        }
        public void ThreadSend(MailMessage msg)
        {
            Thread t = new Thread(delegate()
                {
                    Send(msg);
                });
                t.Start();
        }
        private void ParseAddTo(string to)
        {
            string[] list = to.Split(';');
            for (int i = 0; i < list.Length; i++)
            {
                GetHelper().SendCommand("RCPT To:" + list[i]);
            }
        }
    }
}
