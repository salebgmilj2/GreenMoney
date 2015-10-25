using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Security;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using GM.Models.Public;
using RazorEngine;

namespace GM.Utility.EmailService
{
    public static class EmailService
    {
        public static string SetRecipientEmail(string email)
        {
            bool isProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["IsProductionSite"]);
            var recipientEmail = ConfigurationManager.AppSettings["TestEmailAddress"];
            if (isProduction)
            {
                recipientEmail = email;
            }
            return recipientEmail;
        }


        public static void SendEmailResetPassword(string emailContentPath, UserModel user, string password, string url, MembershipUser membershipUser)
        {
            // email content
            var bodyContent = Razor.Parse(
                System.IO.File.ReadAllText(emailContentPath),
                new
                {
                    FirstName = user.FirstName,
                    Password = password,
                    LoginUrl = url
                }
                );

            var recipientEmail = SetRecipientEmail(membershipUser.Email);

            try
            {

                // create email request
                var request = new SendEmailRequest()
                    .WithDestination(new Destination(new List<string> {recipientEmail}))
                    .WithSource("no-reply@greenmoney.com.au")
                    .WithReturnPath("robert@izilla.com.au")
                    .WithMessage(new Message()
                        .WithSubject(new Content("GreenMoney Password Reset"))
                        .WithBody(new Body().WithHtml(new Content(bodyContent)))
                    );

                // send it
                var client = new AmazonSimpleEmailServiceClient("AKIAIDP5FFSCJUHHC4QA", "NKAzwbtwwhvKuQZj2t6OXxOhaOEuaBYh3E34Jxbs");
                client.SendEmail(request);

            }
            catch (Exception)
            {
 
            }
        }

        public static void SendEmailRegisterInterest(string emailContentPath, string email)
        {
            var recipientEmail = SetRecipientEmail(email);

            // Send email after adding user to interested people
            var bodyContent = new Content(System.IO.File.ReadAllText(emailContentPath));

            // create email request
            var request = new SendEmailRequest()
                .WithDestination(new Destination(new List<string> { recipientEmail }))
                .WithSource("no-reply@greenmoney.com.au")
                .WithReturnPath("robert@izilla.com.au")
                .WithMessage(new Message()
                    .WithSubject(new Content("Thanks for joining the GreenMoney waiting list"))
                    .WithBody(new Body().WithHtml(bodyContent))
                );

            // send it
            var client = new AmazonSimpleEmailServiceClient("AKIAIDP5FFSCJUHHC4QA", "NKAzwbtwwhvKuQZj2t6OXxOhaOEuaBYh3E34Jxbs");
            client.SendEmail(request);
        }

        public static void SendEmailAdditionalMemberInvitation(string email, string inviterName, string url)
        {
            var bodyContent = "Your household member " + inviterName +
                              " sent you this <a href='" + url + "'>link</a> to join Green Money.";

            var recipientEmail = SetRecipientEmail(email);

            // create email request
            var request = new SendEmailRequest()
                .WithDestination(new Destination(new List<string> { recipientEmail }))
                .WithSource("no-reply@greenmoney.com.au")
                .WithReturnPath("robert@izilla.com.au")
                .WithMessage(new Message()
                    .WithSubject(new Content("GreenMoney. Household member invitation"))
                    .WithBody(new Body().WithHtml(new Content(bodyContent)))
                );

            //send it
            var client = new AmazonSimpleEmailServiceClient("AKIAIDP5FFSCJUHHC4QA",
                "NKAzwbtwwhvKuQZj2t6OXxOhaOEuaBYh3E34Jxbs");
            client.SendEmail(request);
        }

        public static void SendEmailVerifyAccount(string emailContentPath, string email, string firstname, string url)
        {
            // email content
            var bodyContent = Razor.Parse(
                System.IO.File.ReadAllText(emailContentPath),
                new
                {
                    FirstName = firstname,
                    VerifyUrl = url
                }
            );

            var recipientEmail = SetRecipientEmail(email);

            // create email request
            var request = new SendEmailRequest()
                .WithDestination(new Destination(new List<string> { recipientEmail }))
                .WithSource("no-reply@greenmoney.com.au")
                .WithReturnPath("robert@izilla.com.au")
                .WithMessage(new Message()
                    .WithSubject(new Content("GreenMoney. Please confirm your registration"))
                    .WithBody(new Body().WithHtml(new Content(bodyContent)))
                );

            // send it
            var client = new AmazonSimpleEmailServiceClient("AKIAIDP5FFSCJUHHC4QA", "NKAzwbtwwhvKuQZj2t6OXxOhaOEuaBYh3E34Jxbs");
            client.SendEmail(request);            
        }


        public static void SendEmailsOnRewardsRedeem(
            string emailContentPath, string emailPathProductOrderConfirmation, string emailPathOrderConfirmation,
            string email, string firstname, string lastname, string url, string voucherUrlBase, CheckoutSubmitModel addToMyWallet)
        {
            foreach (var purchase in addToMyWallet.Purchases)
            {

                if (purchase.NotifyOnRedeem)
                {
                    var recipientEmail = SetRecipientEmail(purchase.PartnerOwnerEmail);
                    string subjectRewardRedeemed = "GreenMoney Reward Redeemed";

                    if (string.IsNullOrWhiteSpace(recipientEmail))
                    {
                        recipientEmail = ConfigurationManager.AppSettings["MasterAdminEmail"];
                        subjectRewardRedeemed = subjectRewardRedeemed + " - no owner email";
                    }

                    var redeem_bodyContent = Razor.Parse(
                        System.IO.File.ReadAllText(emailContentPath),
                        new
                        {
                            Email = email,
                            FirstName = firstname,
                            LastName = lastname,
                            PartnerName = purchase.PartnerName,
                            Name = purchase.Name,
                            Category = purchase.Category,
                            Site = url,
                            VoucherUrl = voucherUrlBase
                        }
                        );

                    // create email request
                    var redeem_request = new SendEmailRequest()
                        .WithDestination(new Destination(new List<string> {recipientEmail}))
                        .WithSource("no-reply@greenmoney.com.au")
                        .WithReturnPath("robert@izilla.com.au")
                        .WithMessage(new Message()
                            .WithSubject(new Content(subjectRewardRedeemed))
                            .WithBody(new Body().WithHtml(new Content(redeem_bodyContent)))
                        );

                    try
                    {
                        var redeem_client = new AmazonSimpleEmailServiceClient("AKIAIDP5FFSCJUHHC4QA", "NKAzwbtwwhvKuQZj2t6OXxOhaOEuaBYh3E34Jxbs");
                        redeem_client.SendEmail(redeem_request);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }


            decimal dollartotal = addToMyWallet.Purchases.Sum(i => i.DollarCost);
            long total = addToMyWallet.Purchases.Sum(i => i.Cost);

            string emailFile;
            if (dollartotal > 0)
                emailFile = emailPathProductOrderConfirmation;
            else
                emailFile = emailPathOrderConfirmation;

            // email content
            var bodyContent = Razor.Parse(
                System.IO.File.ReadAllText(emailFile),
                new
                {
                    FirstName = firstname,
                    Time = DateTime.Now,
                    Total = total,
                    Items = addToMyWallet.Purchases,
                    HasVouchers = addToMyWallet.Purchases.Any(i => i.DollarCost == 0),
                    VoucherUrl = voucherUrlBase
                }
            );

            var recipientEmail1 = SetRecipientEmail(email);

            // create email request
            var request = new SendEmailRequest()
                .WithDestination(new Destination(new List<string> { recipientEmail1 }))
                .WithSource("no-reply@greenmoney.com.au")
                .WithReturnPath("robert@izilla.com.au")
                .WithMessage(new Message()
                    .WithSubject(new Content("GreenMoney Order Confirmation"))
                    .WithBody(new Body().WithHtml(new Content(bodyContent)))
                );

            try
            {
                var client = new AmazonSimpleEmailServiceClient("AKIAIDP5FFSCJUHHC4QA", "NKAzwbtwwhvKuQZj2t6OXxOhaOEuaBYh3E34Jxbs");
                client.SendEmail(request);
            }
            catch (Exception e)
            {
            }
        }
 
    }
}