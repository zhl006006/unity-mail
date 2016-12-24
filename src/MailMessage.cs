using System;
using System.Text;

namespace Qingame.Mail
{
    /// <summary>
    /// </summary>
    public class MailMessage
    {
        public enum BodyFormat
        {
            HTML, TEXT
        }

        private string _from;
        private string _to;
        private string _cc;
        private string _subject;
        private BodyFormat _bodyFormat = BodyFormat.TEXT;
        private string _body;
        private string _fromName;
        private string _toName;

        public string Cc
        {
            get
            {
                return _cc;
            }
            set
            {
                _cc = value;
            }
        }

        public string FromName
        {
            get
            {
                if (string.IsNullOrEmpty(_fromName))
                {
                    _fromName = _from;
                }
                return _fromName;
            }
            set
            {
                _fromName = value;
            }
        }

        public string ToName
        {
            get
            {
                if (string.IsNullOrEmpty(_toName))
                {
                    _toName = _to;
                }
                return _toName;
            }
            set
            {
                _toName = value;
            }
        }

        public string From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
            }
        }

        public string To
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
            }
        }

        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }

        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }
        }

        public BodyFormat MailFormat
        {
            get
            {
                return _bodyFormat;
            }
            set
            {
                _bodyFormat = value;
            }
        }

        public MailMessage()
        {
            
        }
        public MailMessage(string from,string to)
        {
            _from = from;
            _to = to;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("From: {0}\r\n", this.FromName);
            sb.AppendFormat("To: {0}\r\n", this.ToName);
            if (string.IsNullOrEmpty(_cc) == false)
            {
                sb.AppendFormat("Cc: {0}\r\n", _cc.Replace(";", ","));
            }
            sb.AppendFormat("Date: {0}\r\n", DateTime.Now.GetDateTimeFormats('r')[0].ToString());
            sb.AppendFormat("Subject: {0}\r\n", this.Subject);
            sb.AppendFormat("X-Mailer: vmlinux.Net.Mail.SmtpServer\r\n");
            switch (this.MailFormat)
            {
                case BodyFormat.TEXT:
                    sb.AppendFormat("Content-type:text/plain;Charset=utf8\r\n");
                    break;
                case BodyFormat.HTML:
                    sb.AppendFormat("Content-type:text/html;Charset=utf8\r\n");
                    break;
                default:
                    break;
            }
            sb.AppendFormat("\r\n{0}\r\n", this.Body);
            return sb.ToString();
        }
    }
}
