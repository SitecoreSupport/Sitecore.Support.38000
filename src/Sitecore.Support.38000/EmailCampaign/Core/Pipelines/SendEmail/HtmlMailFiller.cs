namespace Sitecore.Support.EmailCampaign.Core.Pipelines.SendEmail
{
  using System;
  using Sitecore.Modules.EmailCampaign.Core.Pipelines;
  using Sitecore.Modules.EmailCampaign.Messages;
  using Diagnostics;
  using Modules.EmailCampaign.Core;
  using Modules.EmailCampaign;
  using StringExtensions;
  using Sitecore.Modules.EmailCampaign.Diagnostics;
  using System.Web;

  public class HtmlMailFiller : MailMessageFiller
  {
    private HtmlMailBase htmlMailBase;

    public HtmlMailFiller(HtmlMailBase message, SendMessageArgs args) : base(message, args)
    {
      this.htmlMailBase = message;
    }

    protected override void AddAttachments()
    {
      DateTime utcNow = DateTime.UtcNow;
      this.AddRelativeAttachments();
      Util.TraceTimeDiff("AddRelativeAttachments", utcNow);
      base.AddAttachments();
    }

    protected void AddRelativeAttachments()
    {
      if (this.htmlMailBase.RelativeFiles != null)
      {
        try
        {
          int numRelatedItems = base.Email.NumRelatedItems;
          foreach (FileInMemory memory in this.htmlMailBase.RelativeFiles)
          {
            base.Email.AddRelatedData2(memory.ByteContent, memory.Source);
            object[] parameters = new object[] { memory.MimeType ?? "text/plain", memory.Name };
            base.Email.AddRelatedHeader(numRelatedItems, "Content-Type", "{0}; name=\"{1}\"".FormatWith(parameters));
            base.Email.AddRelatedHeader(numRelatedItems, "Content-Disposition", "inline; filename=\"{0}\"".FormatWith(new object[] { memory.Name }));
            numRelatedItems++;
          }
        }
        catch (Exception exception)
        {
          this.htmlMailBase.Warnings.Add(exception.Message);
          Logging.LogError(exception);
        }
      }
    }

    protected override void FillBody()
    {
      DateTime utcNow = DateTime.UtcNow;
      base.Email.SetHtmlBody(this.htmlMailBase.ReplaceTokens(this.htmlMailBase.Body));
      Util.TraceTimeDiff("SetEmailHtmlBody(ReplaceTokens(Body))", utcNow);
      utcNow = DateTime.UtcNow;
      base.Email.AddPlainTextAlternativeBody(HttpUtility.HtmlDecode(this.htmlMailBase.ReplaceTokens(this.htmlMailBase.AlternateText)));
      Util.TraceTimeDiff("SetEmailAltBody(ReplaceTokens(AlternateText))", utcNow);
    }
  }











}
