namespace Sitecore.Support.EmailCampaign.Core.Pipelines.SendEmail
{
  using Diagnostics;
  using DynamicSerialization;
  using Modules.EmailCampaign.Core.Pipelines;
  using Modules.EmailCampaign.Factories;
  using Modules.EmailCampaign.Messages;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using Modules.EmailCampaign.Core;

  public class FillEmail
  {
    private MailFiller GetMailFiller(SendMessageArgs args)
    {
      HtmlMailBase ecmMessage = args.EcmMessage as HtmlMailBase;
      if (ecmMessage != null)
      {
        return new HtmlMailFiller(ecmMessage, args);
      }
      MailMessageItem message = args.EcmMessage as MailMessageItem;
      if (message != null)
      {
        return new MailMessageFiller(message, args);
      }
      return null;
    }

    public void Process(SendMessageArgs args)
    {
      if (args.EcmMessage != null)
      {
        MailFiller mailFiller = this.GetMailFiller(args);
        if (mailFiller != null)
        {
          mailFiller.FillEmail();
        }
      }
    }
  }
}