using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Service.Email
{
    public class EmailService : IEmailService
    {
        private readonly ConfigHelper _configuration;

        public EmailService()
        {
            _configuration = new ConfigHelper();
        }
        public bool SendOrderEmail(Stream file, string fileName, Customer customer, string contactName, string email, string orderType)
        {
            try
            {
                var companyName = customer.State == "CA" ? "Perfect Fit West" : "Parts Co";
                var accountingName = customer.State == "CA" ? "Pfwaccounting@perfectfitwest.com" : "Accounting@partscoinc.com";
                var phoneNumber = customer.State == "CA" ? "310-956-4667" : "702-998-8888";
                var subject = customer.State == "CA" ? $"Perfect Fit West Copy of {orderType}" : $"PartsCo Copy of {orderType}";
                var salesAddress = customer.State == "CA" ? _configuration.SalesAddress : _configuration.SalesAddressPartsCo;
                var salesPassword = customer.State == "CA" ? _configuration.SalesPassword : _configuration.SalesPasswordPartsCo;

                StringBuilder body = new StringBuilder();
                if (Regex.IsMatch(customer.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    body.Append($" Dear {customer.CustomerName} and {contactName}, ");
                }
                else
                {
                    body.Append($" Dear {contactName}, ");
                }

                body.Append("\r\n" + "\r\n");
                var ordType = orderType == "RGA" ? orderType : orderType.ToLower();
                body.Append($" Thank you for your business. Please find your {ordType} attached. If you have any questions, please contact our Customer Service. ");
                body.Append("\r\n" + "\r\n");
                body.Append($" Thank you. " + "\r\n");
                body.Append($" {companyName} " + "\r\n");
                body.Append($" {phoneNumber} " + "\r\n");
                body.Append("\r\n" + "\r\n");

                //string to = "noeljreambillo@gmail.com"; //"noeljreambillo @gmail.com; noel_isap@yahoo.com"; 
                string to = $"{customer.Email};{email}";
                string from = salesAddress;
                string smtpHost = _configuration.SMTPHost;
                MailMessage message = new MailMessage();

                //message.To.Add(new MailAddress("noeljreambillo@gmail.com"));
                //message.To.Add(new MailAddress("noel_isap@yahoo.com"));
                if (Regex.IsMatch(customer.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    message.To.Add(new MailAddress(customer.Email));
                }

                message.To.Add(new MailAddress(email));
                message.Subject = subject;
                message.From = new MailAddress(from);
                message.Body = body.ToString();

                Attachment data = new Attachment(file, fileName);
                message.Attachments.Add(data);

                SmtpClient smtp = new SmtpClient(smtpHost);
                smtp.Port = 587;
                //smtp.Credentials = new System.Net.NetworkCredential("sales@perfectfitwest.com", "pfw@parts1");
                smtp.Credentials = new System.Net.NetworkCredential(salesAddress, salesPassword);
                smtp.EnableSsl = true;
                System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;

                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                //eventLog.Dispose();
            }
        }

        public bool SendOrderEmailByContacts(Stream file, string fileName, Customer customer, List<Contact> contacts, string orderType)
        {
            try
            {
                var companyName = customer.State == "CA" ? "Perfect Fit West" : "Parts Co";
                var accountingName = customer.State == "CA" ? "Pfwaccounting@perfectfitwest.com" : "Accounting@partscoinc.com";
                var phoneNumber = customer.State == "CA" ? "310-956-4667" : "702-998-8888";
                var subject = customer.State == "CA" ? $"Perfect Fit West Copy of {orderType}" : $"PartsCo Copy of {orderType}";
                var salesAddress = customer.State == "CA" ? _configuration.SalesAddress : _configuration.SalesAddressPartsCo;
                var salesPassword = customer.State == "CA" ? _configuration.SalesPassword : _configuration.SalesPasswordPartsCo;

                StringBuilder body = new StringBuilder();
                body.Append($" Dear");

                foreach (var contact in contacts)
                {
                    if (Regex.IsMatch(contact.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                    {
                        body.Append($" {contact.ContactName},");
                    }
                }

                body.Append("\r\n" + "\r\n");
                var ordType = orderType == "RGA" ? orderType : orderType.ToLower();
                body.Append($" Thank you for your business. Please find your {ordType} attached. If you have any questions, please contact our Customer Service. ");
                body.Append("\r\n" + "\r\n");
                body.Append($" Thank you. " + "\r\n");
                body.Append($" {companyName} " + "\r\n");
                body.Append($" {phoneNumber} " + "\r\n");
                body.Append("\r\n" + "\r\n");

                //string to = "noeljreambillo@gmail.com"; //"noeljreambillo @gmail.com; noel_isap@yahoo.com"; 
                string smtpHost = _configuration.SMTPHost;
                string from = salesAddress;

                MailMessage message = new MailMessage();

                foreach (var contact in contacts)
                {
                    if (contact.Email != null)
                    {
                        if (Regex.IsMatch(contact.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                        {
                            message.To.Add(new MailAddress(contact.Email));
                        }
                    }
                }

                //message.To.Add(new MailAddress("noeljreambillo@gmail.com"));
                //message.To.Add(new MailAddress("noel_isap@yahoo.com"));

                message.Subject = subject;
                message.From = new MailAddress(from);
                message.Body = body.ToString();

                Attachment data = new Attachment(file, fileName);
                message.Attachments.Add(data);

                SmtpClient smtp = new SmtpClient(smtpHost);
                smtp.Port = 587;
                //smtp.Credentials = new System.Net.NetworkCredential("sales@perfectfitwest.com", "pfw@parts1");
                smtp.Credentials = new System.Net.NetworkCredential(salesAddress, salesPassword);
                smtp.EnableSsl = true;
                System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;

                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                //eventLog.Dispose();
            }
        }

        public bool SendStatementEmail(Stream file, string fileName, Customer customer, DateTime statementDate, List<Contact> contacts)
        {
            try
            {
                /*
             *  To: customer@email.com
                Subject: Your account statement from [Your Company]

                Hello [Customer Name],

                Thank you for being a valued customer of [Your Company]. 
                We appreciate your business and want to ensure that you are always up-to-date on what you owe.
                Please find attached your latest statement of account for the period [Date] to [Date]. 
                If you have any questions, please do not hesitate to contact us.

                Thank you for your time, and have a great day!
                Sincerely, 
                [Your Company] [Your Name]
             */

                //var contacts = await _dataContext.Contacts.Where(e => e.CustomerId == customer.Id && e.IsEmailStatement).ToListAsync();

                if (contacts.Any())
                {
                    var companyName = customer.State == "CA" ? "Perfect Fit West" : "Parts Co";
                    var accountingName = customer.State == "CA" ? "Pfwaccounting@perfectfitwest.com" : "Accounting@partscoinc.com";
                    var phoneNumber = customer.State == "CA" ? "310-956-4667" : "702-998-8888";
                    var supportAddress = _configuration.SupportAddress;
                    var supportPassword = _configuration.SupportPassword;

                    StringBuilder body = new StringBuilder();
                    body.Append($" Hello {customer.CustomerName}, ");
                    body.Append("\r\n" + "\r\n");
                    body.Append($" Thank you for being a valued customer of {companyName}. " + "\r\n");
                    body.Append(" We appreciate your business and want to ensure that you are always up-to-date on what you owe. " + "\r\n");
                    body.Append($" Please find attached your latest statement of account ending date {statementDate.ToString("dddd, dd MMMM yyyy")}. " + "\r\n");
                    body.Append(" If you have any questions, please do not hesitate to contact us. " + "\r\n" + "\r\n");
                    body.Append(" Thank you for your time, and have a great day! " + "\r\n" + "\r\n");
                    body.Append(" Sincerely, " + "\r\n");
                    body.Append($" {companyName} " + "\r\n");
                    body.Append($" {accountingName} " + "\r\n");
                    body.Append($" {phoneNumber} " + "\r\n");
                    body.Append("\r\n" + "\r\n");

                    //string to = "noeljreambillo@gmail.com"; //"noeljreambillo @gmail.com; noel_isap@yahoo.com"; 
                    string subject = $"Your account statement from {companyName}";
                    string from = supportAddress;
                    string smtpHost = _configuration.SMTPHost;
                    MailMessage message = new MailMessage();

                    foreach (var contact in contacts.Where(e => e.IsEmailStatement).ToList())
                    {
                        if (Regex.IsMatch(contact.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                        {
                            message.To.Add(new MailAddress(contact.Email));
                        }
                    }

                    if (message.To.Count > 0)
                    {
                        message.Subject = subject;
                        message.From = new MailAddress(from);
                        message.Body = body.ToString();

                        Attachment data = new Attachment(file, fileName);
                        message.Attachments.Add(data);

                        if (smtpHost != null)
                        {
                            SmtpClient smtp = new SmtpClient(smtpHost);
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential(supportAddress, supportPassword);
                            smtp.EnableSsl = true;
                            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;

                            smtp.Send(message);
                            return true;
                        }

                        return false;
                    }

                    return false;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                //eventLog.Dispose();

            }
        }

        public bool SendUserNotificationEmail(List<UserNotificationEmailDTO> userNotificationEmails)
        {
            /*
             * Email Subject : Skipped - Part(s) Order Number 363539
             * 
             * Hello User  “CreatedBy”,
             * 
             * System Message :  Part Number GM24ER got Skipped on Order 363539
             * 
             * DO NOT REPLY ON THIS EMAIL.
             * Perfect Fit West
             */

            try
            {
                //var user = await _dataContext.Users.FirstOrDefaultAsync(e => e.UserName == userNotificationEmailDTO.Username);

                foreach(var userNotificationEmail in userNotificationEmails)
                {
                    var salesAddress = _configuration.SalesAddress;
                    var salesPassword = _configuration.SalesPassword;

                    StringBuilder body = new StringBuilder();
                    body.Append($" Hello User {userNotificationEmail.Username}, ");
                    body.Append("\r\n" + "\r\n");
                    body.Append($" System Message: Part Number {userNotificationEmail.PartNumber} got {userNotificationEmail.Subject} on Order {userNotificationEmail.OrderNumber}. " + "\r\n");
                    body.Append("\r\n" + "\r\n");
                    body.Append(" DO NOT REPLY TO THIS EMAIL. " + "\r\n");
                    body.Append($" Perfect Fit West " + "\r\n");
                    body.Append("\r\n" + "\r\n");

                    //string to = "noeljreambillo@gmail.com"; //"noeljreambillo @gmail.com; noel_isap@yahoo.com"; 
                    string subject = $"{userNotificationEmail.Subject} - Part(s) Order Number {userNotificationEmail.OrderNumber}";
                    string from = salesAddress;
                    string smtpHost = _configuration.SMTPHost;
                    MailMessage message = new MailMessage();

                    if (Regex.IsMatch(userNotificationEmail.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                    {
                        message.To.Add(new MailAddress(userNotificationEmail.Email));
                    }
                    //message.To.Add(new MailAddress("noeljreambillo@gmail.com"));

                    if (message.To.Count > 0)
                    {
                        message.Subject = subject;
                        message.From = new MailAddress(from);
                        message.Body = body.ToString();

                        if (smtpHost != null)
                        {
                            SmtpClient smtp = new SmtpClient(smtpHost);
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential(salesAddress, salesPassword);
                            smtp.EnableSsl = true;
                            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;

                            smtp.Send(message);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                //eventLog.Dispose();

            }
        }

        public bool SendUserRegistrationEmail(RegisterUserDTO user)
        {
            try
            {
                var salesAddress = _configuration.SalesAddress;
                var salesPassword = _configuration.SalesPassword;

                StringBuilder body = new StringBuilder();
                body.Append(" A New Account has been registered to Perfect Fit Website ");
                body.Append("\r\n" + "\r\n");
                body.Append(" Company Name   : " + user.CustomerName.Trim() + "\r\n");
                body.Append(" Location Name  : " + user.LocationName.Trim() + "\r\n");
                body.Append(" Address        : " + user.AddressLine1.Trim() + (string.IsNullOrWhiteSpace(user.AddressLine2) ? "" : user.AddressLine2.Trim()) + "\r\n");
                body.Append(" City           : " + user.City.Trim() + "\r\n");
                body.Append(" State          : " + user.State.Trim() + "\r\n");
                body.Append(" Zipcode        : " + user.ZipCode.Trim() + "\r\n");
                body.Append(" Phone          : " + user.PhoneNumber.Trim() + "\r\n");
                body.Append(" Contact Name   : " + user.ContactName.Trim() + "\r\n");
                body.Append(" Email Address  : " + user.Email.Trim() + "\r\n");
                body.Append(" Username       : " + user.UserName.Trim() + "\r\n");
                body.Append(" Password       : " + user.Password.Trim() + "\r\n");
                body.Append("\r\n" + "\r\n");

                string to = salesAddress; // _configuration.GetSection("AppSettings:SupportAddress").Value;
                string subject = "Perfect Fit B2B Account";
                string from = salesAddress; // _configuration.GetSection("AppSettings:SupportAddress").Value;
                string smtpHost = _configuration.SMTPHost;
                MailMessage message = new MailMessage();
                message.To.Add(to);
                message.Subject = subject;
                message.From = new MailAddress(from);
                message.Body = body.ToString();
                SmtpClient smtp = new SmtpClient(smtpHost);
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential(salesAddress, salesPassword);
                smtp.EnableSsl = true;
                System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;

                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                //eventLog.Dispose();
            }
        }
    }
}
