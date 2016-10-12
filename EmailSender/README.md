## Email Sender

It might be one of the simplest usage scenarios, but it is also one of the most useful: Queueing up
work to be handled in a reliable fashion, with retries and error queues and everything :)

In this case, it's about sending emails. The `SmtpClient` is injected into `SendEmailHandler`,
and it might be badly configured (like e.g. using the wrong port on the SMTP server, missing
configuration, etc.), which is why it is good to have a message queue in front of the actual
sending of the mails.