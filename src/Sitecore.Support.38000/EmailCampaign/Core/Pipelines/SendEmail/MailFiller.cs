namespace Sitecore.Support.EmailCampaign.Core.Pipelines.SendEmail
{
  using Chilkat;
  using Data.Items;
  using Modules.EmailCampaign;
  using Modules.EmailCampaign.Core;
  using Sitecore.Diagnostics;
  using Sitecore.Modules.EmailCampaign.Core.Pipelines;
  using Sitecore.Modules.EmailCampaign.Messages;
  using System;
  using System.Net;
  using System.Web;
  using Text;

  public abstract class MailFiller
  {
    private IMailMessage mailMessage;

    protected MailFiller(IMailMessage message, SendMessageArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(message, "message");
      this.Args = args;
      this.mailMessage = message;
      this.Email = new Email();
    }

    protected virtual void AddAttachments()
    {
      DateTime utcNow = DateTime.UtcNow;
      if (this.mailMessage.Attachments != null)
      {
        foreach (MediaItem item in this.mailMessage.Attachments)
        {
          FileInMemory mediaItemContent = ItemUtilExt.GetMediaItemContent(item);
          this.Email.AddDataAttachment(mediaItemContent.FullName, mediaItemContent.ByteContent);
        }
        Util.TraceTimeDiff("AddAttachments", utcNow);
      }
    }

    protected virtual void FillBody()
    {
      DateTime utcNow = DateTime.UtcNow;
      this.Email.Body = this.mailMessage.ReplaceTokens(this.mailMessage.Body);
      Util.TraceTimeDiff("SetEmailBody(ReplaceTokens(Body))", utcNow);
    }

    public void FillEmail()
    {
      DateTime utcNow = DateTime.UtcNow;
      this.Email = new Email();
      this.Email.FromAddress = this.mailMessage.FromAddress;
      if (!string.IsNullOrEmpty(this.mailMessage.FromName))
      {
        this.Email.FromName = HttpUtility.HtmlDecode(this.mailMessage.FromName);
      }
      if (!string.IsNullOrEmpty(this.mailMessage.ReplyTo))
      {
        this.Email.ReplyTo = this.mailMessage.ReplyTo;
      }
      this.FillHeaderFields();
      Util.TraceTimeDiff("Fill simple Chilkat.Email fields", utcNow);
      DateTime start = DateTime.UtcNow;
      this.AddAttachments();
      DateTime end = DateTime.UtcNow;
      DateTime startTime = end;
      this.Email.Subject = HttpUtility.HtmlDecode(this.mailMessage.ReplaceTokens(this.mailMessage.Subject));
      Util.TraceTimeDiff("SetEmailSubject(ReplaceTokens(Subject))", startTime);
      this.FillBody();
      DateTime time5 = DateTime.UtcNow;
      this.Email.ClearTo();
      if (!string.IsNullOrEmpty(this.mailMessage.To))
      {
        ListString str = new ListString(this.mailMessage.To, ',');
        foreach (string str2 in str)
        {
          this.Email.AddTo(string.Empty, str2.Trim());
        }
      }
      Util.TraceTimeDiff("Add To(s) to email", time5);
      DateTime time6 = DateTime.UtcNow;
      this.Args.CustomData["Email"] = this.Email;
      this.Args.GenerateMimeTime = Util.GetTimeDiff(utcNow, time6);
      this.Args.InsertFilesTime = Util.GetTimeDiff(start, end);
      this.Args.ReplaceTokensTime = Util.GetTimeDiff(startTime, time5);
      if (!string.IsNullOrEmpty(this.mailMessage.Charset))
      {
        this.Email.Charset = this.mailMessage.Charset;
      }
    }

    protected abstract void FillHeaderFields();

    protected SendMessageArgs Args { get; set; }

    protected Email Email { get; set; }
  }






}
