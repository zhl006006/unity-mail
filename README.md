# unity-mail
unity发送邮件系统
<br>
<br>

```c#

SmtpServer smtp = new SmtpServer('smtp.exmail.mail.com');
smtp.Timeout = 6000;
smtp.RequireAuthorization = true;
smtp.SetCredentials("from@mail.com", "password");
MailMessage msg = new MailMessage("from@mail.com", "to@mail.com");
msg.Subject = "title";
msg.Body = "body";
smtp.ThreadSend(msg);

```
