
namespace Sitecore.Support.EmailCampaign.Core.Pipelines.SendEmail
{
  using System;
  using Sitecore.Modules.EmailCampaign.Core.Pipelines;
  using Sitecore.Modules.EmailCampaign.Messages;
  using Data;
  using Sitecore.Modules.EmailCampaign.Diagnostics;

  public class MailMessageFiller : MailFiller
  {
    private readonly MailMessageItem mailMessageItem;

    public MailMessageFiller(MailMessageItem message, SendMessageArgs args) : base(message, args)
    {
      this.mailMessageItem = message;
    }

    protected override void FillHeaderFields()
    {
      if (!string.IsNullOrEmpty(this.mailMessageItem.ReturnPath))
      {
        base.Email.AddHeaderField("Return-Path", this.mailMessageItem.ReturnPath);
      }
      Guid id = (this.mailMessageItem.RecipientId != Guid.Empty) ? this.mailMessageItem.EncryptedContactId : Guid.Empty;
      if (id != Guid.Empty)
      {
        base.Email.AddHeaderField("X-Sitecore-Campaign", new ShortID(id).ToString());
      }
      base.Email.AddHeaderField("X-MessageID", id.ToString());
      base.Email.AddHeaderField("X-BatchID", this.mailMessageItem.ID);
    }
  }






}