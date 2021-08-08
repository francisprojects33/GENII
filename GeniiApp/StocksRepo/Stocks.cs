using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using GeniiApp.Mail;
using GeniiApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using GeniiApp.Data;

namespace GeniiApp.StocksRepo
{
    public class Stocks : IStocks
    {
        private readonly IEmailConfiguration _emailConfig;
        private UserManager<ApplicationUser> _userManager;

        Dictionary<int, string> dictIsProductItemsLow;

        public Stocks()
        { }

        public Stocks(IEmailConfiguration emailConfig, UserManager<ApplicationUser> userManager)
        {
            _emailConfig = emailConfig;
            _userManager = userManager;
        }

        public async Task<bool> IsStocksLevelLowAsync(IList<StockProductItems> stockItems)
        {
            bool isStocksLows = false;
            dictIsProductItemsLow = new Dictionary<int, string>();

            int initialProductsQuantity;
            int soldProductsQuantity;

            foreach (var item in stockItems)
            {
                initialProductsQuantity = item.ProductItemInitialQuantity;
                soldProductsQuantity = item.ProductItemSoldQuantity;

                double find30percentOfStocks = initialProductsQuantity * 0.3;
                int remainingStocks = initialProductsQuantity - soldProductsQuantity;

                // Still have stocks to sell.
                if (remainingStocks > (int)find30percentOfStocks)
                {
                    isStocksLows = false;
                }

                // Stocks are running low, need to restock now.
                if (remainingStocks <= (int)find30percentOfStocks)
                {
                    isStocksLows = true;
                    dictIsProductItemsLow.Add(item.ProductItemId, item.ProductItemName);
                }
            }

            if (dictIsProductItemsLow.Any())
            {
                await SendNotificationsAsync();
            }

            return await Task.FromResult(isStocksLows);
        }

        public async Task<string> SendNotificationsAsync()
        {
            MimeMessage message = new MimeMessage();
            var report = new StringBuilder();
            int index = 1;

            foreach (var item in dictIsProductItemsLow)
            {
                report.AppendLine($"{index + ". " + item.Value + "\n"}");
                index++;
            }

            string ItemsReport = report.ToString();
            var managerEmails = await GetManagerEmailsAsync();

            InternetAddressList list = new InternetAddressList();

            foreach (var item in managerEmails)
            {
                list.Add(new MailboxAddress("", item));
            }

            message.From.Add(new MailboxAddress("Items Restock", _emailConfig.SmtpUsername));
            message.To.AddRange(list);
            message.Subject = "Stock Items are running Low";

            string title = "\nThe following items need to be restocked\n";
            string titleUnderline = "-------------------------------------------------------\n\n";
            string mydata = title + titleUnderline + ItemsReport;

            message.Body = new TextPart("plain")
            {
                Text = mydata
            };

            #region SMTP_Details

            SmtpClient client = new SmtpClient();

            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.SmtpPort, true);
                client.Authenticate(_emailConfig.SmtpUsername, _emailConfig.SmtpPassword);
                client.Send(message);
                Debug.WriteLine("Email Sent!.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }

            #endregion

            return "Completed";
        }

        private async Task<List<String>> GetManagerEmailsAsync()
        {
            var userNames = await _userManager.GetUsersInRoleAsync("MANAGER");
            var userEmails = userNames.Select(e => e.Email).ToList();
            return userEmails;
        }

    }
}
