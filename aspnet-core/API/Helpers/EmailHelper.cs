using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Infrastucture;
using Microsoft.EntityFrameworkCore;
using Service.Email;
using System.Text;

namespace API.Helpers
{
    public class EmailHelper
    {
        private readonly DataContext _dataContext;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public EmailHelper(DataContext dataContext, IEmailService emailService, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _emailService = emailService;
            _configuration = configuration;
        }

        public bool SendUserNotificationEmail(List<UserNotificationEmailDTO> userNotificationEmails)
        {
            return _emailService.SendUserNotificationEmail(userNotificationEmails);
        }

        public async Task<bool> SendOrderEmail(int orderId, string contactName, string email)
        {
            var result = false;
            try
            {
                string _ironPdfLicenseKey = _configuration.GetSection("AppSettings:IronPdf.LicenseKey").Value; //System.Configuration.ConfigurationManager.AppSettings["IronPdf.LicenseKey"];
                IronPdf.License.LicenseKey = _ironPdfLicenseKey;
                var lic = IronPdf.License.IsLicensed;

                // Get Order
                var order = await _dataContext.Orders.FirstOrDefaultAsync(e => e.Id == orderId);

                if (order != null)
                {
                    var orderDetails = await _dataContext.OrderDetails.Where(e => e.OrderId == orderId && e.IsActive && !e.IsDeleted).ToListAsync();
                    var customer = await _dataContext.Customers.FirstOrDefaultAsync(e => e.Id == order.CustomerId);
                    var orderType = order.OrderStatusId == 9 ? "RGA" : order.OrderStatusId == 5 ? "Credit Memo" : order.IsQuote ? "Quote" : "Invoice";
                    var folderName = Path.Combine("Assets");
                    var cssPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                    var renderer = new IronPdf.ChromePdfRenderer();
                    renderer.RenderingOptions.CustomCssUrl = cssPath + "\\orderstyle.css";
                    renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.Custom;
                    renderer.RenderingOptions.SetCustomPaperSizeInInches(8.5, 11);

                    if (customer != null)
                    {
                        var htmlStr = await GenerateOrderAttachment(order, orderDetails, customer);
                        var pdf = renderer.RenderHtmlAsPdf(@htmlStr);
                        //pdf.SaveAs("C:/PerfectFitWest/aspnet-core/API/Resources/" + $"Invoice-{order.OrderNumber}.pdf");

                        MemoryStream attachment = pdf.Stream;
                        result = _emailService.SendOrderEmail(attachment, $"Invoice-{order.OrderNumber}.pdf", customer, contactName, email, orderType);
                    }
                }
                
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }

        public async Task<bool> SendOrderEmailByContacts(Order order, List<OrderDetail> orderDetails, Customer customer, List<Contact> contacts)
        {
            var result = false;
            try
            {
                string _ironPdfLicenseKey = _configuration.GetSection("AppSettings:IronPdf.LicenseKey").Value; //System.Configuration.ConfigurationManager.AppSettings["IronPdf.LicenseKey"];
                IronPdf.License.LicenseKey = _ironPdfLicenseKey;
                var lic = IronPdf.License.IsLicensed;

                var orderType = order.OrderStatusId == 9 ? "RGA" : order.OrderStatusId == 5 ? "Credit Memo" : order.IsQuote ? "Quote" : "Invoice";
                var folderName = Path.Combine("Assets");
                var cssPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                var renderer = new IronPdf.ChromePdfRenderer();
                renderer.RenderingOptions.CustomCssUrl = cssPath + "\\orderstyle.css";
                renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.Custom;
                renderer.RenderingOptions.SetCustomPaperSizeInInches(8.5, 11);

                var htmlStr = await GenerateOrderAttachment(order, orderDetails, customer);
                var pdf = renderer.RenderHtmlAsPdf(@htmlStr);
                //pdf.SaveAs("C:/PerfectFitWest/aspnet-core/API/Resources/" + $"Invoice-{order.OrderNumber}.pdf");

                MemoryStream attachment = pdf.Stream;
                result = _emailService.SendOrderEmailByContacts(attachment, $"Invoice-{order.OrderNumber}.pdf", customer, contacts, orderType);

                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }

        public async Task<bool> SendStatementEmail(Customer customer, StatementReportDTO statement, DateTime reportDate, DateTime dueDate)
        {
            var result = false;
            try
            {
                string _ironPdfLicenseKey = _configuration.GetSection("AppSettings:IronPdf.LicenseKey").Value; //System.Configuration.ConfigurationManager.AppSettings["IronPdf.LicenseKey"];
                IronPdf.License.LicenseKey = _ironPdfLicenseKey;
                var lic = IronPdf.License.IsLicensed;

                var folderName = Path.Combine("Assets");
                var cssPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                var renderer = new IronPdf.ChromePdfRenderer();
                renderer.RenderingOptions.CustomCssUrl = cssPath + "\\style.css";
                renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.Custom;
                renderer.RenderingOptions.SetCustomPaperSizeInInches(8.5, 11);

                var htmlStr = await GenerateStatementAttachment(customer, statement, reportDate, dueDate);
                var pdf = renderer.RenderHtmlAsPdf(@htmlStr);
                //pdf.SaveAs("C:/PerfectFitWest/aspnet-core/API/Resources/" + $"{customer.CustomerName}(SOA).pdf");

                MemoryStream attachment = pdf.Stream;

                var contacts = await _dataContext.Contacts.Where(e => e.CustomerId == customer.Id && e.IsEmailStatement).ToListAsync();
                result = _emailService.SendStatementEmail(attachment, $"{customer.CustomerName}(SOA).pdf", customer, reportDate, contacts);

                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }

        private async Task<string> GenerateOrderAttachment(Order order, List<OrderDetail> orderDetails, Customer customer)
        {
            var folderName = Path.Combine("Assets", "img");
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            // Prepare Variables
            var companyName = customer.State == "CA" ? "Perfect Fit West LLC" : "PartsCo Inc.";
            var address = order.BillState == "CA" ? "3383 OLIVE AVE, SIGNAL HILL CA 90755" : "5151 W Oquendo Rd, Las Vegas, NV 89118";
            var website = order.BillState == "CA" ? "PERFECTFITWEST.COM" : "PartsCoInc.com";
            var phoneNumber = customer.State == "CA" ? "+1 3109564667" : "702-998-8888";

            var address2 = customer.State == "CA" ? "Signal Hill, CA 90755" : "Las Vegas, NV 89118";
            var webSite = customer.State == "CA" ? "sales@perfectfitwest.com" : "PartsCoInc.com";
            var pngBinaryData = System.IO.File.ReadAllBytes(customer.State == "CA" ? $"{logoPath}\\pfitwest.png" : $"{logoPath}\\partsco.jpg");
            var imgDataURI = @"data:image/png;base64," + Convert.ToBase64String(pngBinaryData);

            var sb = new StringBuilder();
            sb.Append(@"<html>");
            //sb.Append(@"<head>");
            //sb.Append(@"<style>body {font-family: Tahoma; font-size: medium}</style>");
            //sb.Append(@"<style>body {font-family: Tahoma; font-size: large}</style>");
            //sb.Append(@"</head>");

            sb.Append($"<body id='section-print' style='margin-top: 20px;'>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-left'>");
            sb.Append($"<img src={imgDataURI} style='width: 275px;'>");
            sb.Append($"</div>");

            if (!order.IsQuote)
            {
                sb.Append($"<div class='flex-child-center-fl'>");
                if (order.OrderStatusId == 9)
                {
                    sb.Append($"<div class='flex-parent'>");
                    sb.Append($"<div class='flex-child-center-fl-new'>");
                    sb.Append($"<span class='flex-child-center-fl-new'>RETURN GOOD AUTHORIZATION</span>");
                    sb.Append($"</div>");
                    sb.Append($"</div>");
                    sb.Append($"<div class='flex-parent'>");
                    sb.Append($"<div class='flex-child-center-fl'>");
                    sb.Append($"<span class='flex-child-center-fl'>PFOS RGA NUMBER: {order.OrderNumber}</span>");
                    sb.Append($"</div>");
                    sb.Append($"</div>");
                    sb.Append($"<div class='flex-parent'>");
                    sb.Append($"<div class='flex-child-center-fl-wide'>");
                    sb.Append($"<span class='flex-child-center-fl'>THIS IS NOT A CREDIT, CREDIT WILL FOLLOW</span>");
                    sb.Append($"</div>");
                    sb.Append($"</div>");
                }
                else if (order.OrderStatusId == 5)
                {
                    sb.Append($"<span>PFOS CREDIT MEMO: {order.OrderNumber}</span>");
                }
                else
                {
                    var ordType = order.IsQuote ? "QUOTE" : "ORDER";
                    var ordNumber = order.IsQuote ? order.QuoteNumber : order.OrderNumber;

                    sb.Append($"<span>PFOS {ordType} NUMBER: {ordNumber}</span>");
                }
                sb.Append($"</div>");
            }
            else
            {
                sb.Append($"<div class='flex-child-center-l'>PFOS QUOTE NUMBER: {order.QuoteNumber}</div>");
            }

            if (order.OrderStatusId == 9)
            {
                sb.Append($"<div class='flex-child-right'>");
                sb.Append($"<div style='text-align: center; font-weight: bolder; font-size: xx-large; padding-left: 75px;'>RGA</div>");
                sb.Append($"</div>");
            }
            else 
            {
                sb.Append($"<div class='flex-child-right'>SALES ORDER: {order.InvoiceNumber}");
                sb.Append($"</div>");
            }

            //if (!order.IsQuote)
            //{
            //    //sb.Append($"<div>");
            //    //sb.Append($"<ngx-barcode [bc-value]='order.orderNumber' [bc-height]='25' [bc-width]='2' [bc-margin-left]='50' [bc-display-value]='false'></ngx-barcode>");
            //    //sb.Append($"</div>");
            //}
            
            sb.Append($"</div>");

            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-left'>{address}</div>");
            sb.Append($"<div class='flex-child-right'>PRINTED DATE/TIME: {order.OrderDate.ToString("MM/dd/yyyy h:mm tt")}</div>");
            sb.Append($"</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-left'>TEL: {phoneNumber}</div>");
            sb.Append($"<div class='flex-child-right'>WEBSITE: {website}</div>");
            sb.Append($"</div>");
            sb.Append($"<div class='flex-divider'>___________________________________________________________________________________________________________________</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-left-l'>SOLD TO: {order.CustomerName}</div>");
            sb.Append($"<div class='flex-child-left-l'>SHIP TO: {order.ShipAddressName}</div>");
            sb.Append($"</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-left-l'>{order.BillAddress}</div>");
            sb.Append($"<div class='flex-child-left-l'>{order.ShipAddress}</div>");
            sb.Append($"</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-left-l'>{$"{order.BillCity} {order.BillState} {order.BillZipCode}"}</div>");
            sb.Append($"<div class='flex-child-left-l'>{$"{order.ShipCity} {order.ShipState} {order.ShipZipCode}"}</div>");
            sb.Append($"</div>");
            sb.Append($"<div class='flex-divider'>___________________________________________________________________________________________________________________</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-center'>ACCOUNT</div>");
            sb.Append($"<div class='flex-child-center'>PHONE</div>");
            sb.Append($"<div class='flex-child-center'>PO/RO</div>");
            sb.Append($"<div class='flex-child-center'>TERMS</div>");
            sb.Append($"<div class='flex-child-center'>SOLD BY</div>");
            sb.Append($"</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-center'>{order.AccountNumber}</div>");
            sb.Append($"<div class='flex-child-center'>{order.PhoneNumber}</div>");
            sb.Append($"<div class='flex-child-center'>{order.PurchaseOrderNumber}</div>");
            sb.Append($"<div class='flex-child-center'>{order.PaymentTermName}</div>");
            sb.Append($"<div class='flex-child-center'>{order.CreatedBy}</div>");
            sb.Append($"</div>");
            sb.Append($"<div class='flex-divider'>___________________________________________________________________________________________________________________</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-center-h-70'>QTY</div>");
            sb.Append($"<div class='flex-child-center-h-150'>ITEM NO</div>");
            sb.Append($"<div class='flex-child-center-h'>DESCRIPTION-PARTSLINK-LOCATION</div>");
            
            if (order.OrderStatusId != 9)
            {
                sb.Append($"<div class='flex-child-center-h-100'>LIST</div>");
            }
            
            sb.Append($"<div class='flex-child-center-h-100'>PRICE</div>");
            sb.Append($"<div class='flex-child-center-h-100'>EXT</div>");
            sb.Append($"</div>");

            foreach (var item in orderDetails)
            {
                var stock = await _dataContext.WarehouseStocks.FirstOrDefaultAsync(e => e.ProductId == item.ProductId && e.Quantity > 0);
                var description = $"{item.YearFrom}-{item.YearTo} {item.PartDescription}, {item.MainPartsLinkNumber}, ";
                if (item.OnHandQuantity > 0 && stock != null)
                {
                    description += stock.Location + ", STOCK";
                }
                else
                {
                    description += $"{item.VendorPartNumber}, {item.VendorCode}";
                }

                sb.Append($"<div class='flex-parent-padded'>");
                sb.Append($"<div class='flex-child-center-70'>{item.OrderQuantity}&nbsp;&nbsp;&nbsp;{item.OrderQuantity}</div>");
                sb.Append($"<div class='flex-child-left-l-150'>{item.PartNumber}</div>");
                sb.Append($"<div class='flex-child-left'>{description}</div>");
                
                if (order.OrderStatusId != 9)
                {
                    sb.Append($"<div class='flex-child-right-100'>{item.ListPrice.ToString("0.00")}</div>");
                }
                
                sb.Append($"<div class='flex-child-right-100'>{item.WholesalePrice.ToString("0.00")}</div>");
                sb.Append($"<div class='flex-child-right-100'>{item.TotalAmount.ToString("0.00")}</div>");
                sb.Append($"</div>");
            }

            sb.Append($"<br>");

            if (order.OrderStatusId == 9)
            {
                sb.Append($"<div>");
                sb.Append($"<div class='flex-parent>");
                var rgaReason = GetRGAReason(order.RGAReason);
                sb.Append($"<div class='flex-child-left'>RETURN REASON: {rgaReason}</div>");
                sb.Append($"<div>");
                sb.Append($"<div class='flex-parent'>");
                sb.Append($"<div class='flex-child-left'>NOTES: {order.RGAReasonNotes}</div>");
                sb.Append($"</div>");
                sb.Append($"<div class='flex-parent'>");
                var rgaType = order.RGAType == 1 ? "Incoming" : "Outgoing";
                sb.Append($"<div class='flex-child-left'>RGA TYPE:<span style='font-size: xx-large; font-weight: bold;'> {rgaType}</span></div>");
                sb.Append($"</div>");
                sb.Append($"</div>");
            }

            sb.Append($"<div>TOTAL QUANTITY: {orderDetails.Count}</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-left'>NOTES: {order.OrderedByNotes}</div>");
            sb.Append($"</div>");
            sb.Append($"<div class='flex-parent'>");
            
            if (order.OrderStatusId == 9)
            {
                sb.Append($"<div class='flex-child-left'></div>");
            }
            else
            {
                sb.Append($"<div class='flex-child-left'>ORDERED BY: {order.OrderedBy}</div>");
            }
            
            sb.Append($"<div class='flex-child-left'>PHONE: {order.OrderedByPhoneNumber}</div>");
            sb.Append($"</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-left'>RECEIVED BY: _______________________</div>");
            sb.Append($"<div class='flex-child-left'>PAYMENT TYPE: _______________________</div>");
            sb.Append($"</div>");

            var orderType = order.DeliveryType == 1 ? "DELIVERY" : order.DeliveryType == 2 ? "PICK UP" : "SHIPPING";
            sb.Append($"<div style='font-size: x-large; font-weight: bolder;'>{orderType} {order.ShipZone}</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-left'>EXPECTED DELIVERY DATE: {order.DeliveryDate}</div>");
            sb.Append($"<div class='flex-child-right-l-150'>SUB TOTAL:</div>");
            sb.Append($"<div class='flex-child-right-l-75'>{order.SubTotalAmount.Value.ToString("0.00")}</div>");
            sb.Append($"</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-left'>20% RESTOCKING FEE AFTER 10 DAYS, NO RETURNS AFTER 30 DAYS</div>");
            sb.Append($"<div class='flex-child-right-l-150'>TAX:</div>");
            sb.Append($"<div class='flex-child-right-l-75'>{order.TotalTax.Value.ToString("0.00")}</div>");
            sb.Append($"</div>");
            sb.Append($"<div class='flex-parent'>");
            sb.Append($"<div class='flex-child-left'>50% RESTOCKING FEE FOR NO BAG/BOX ITEMS</div>");
            sb.Append($"<div class='flex-child-right-l-150'>TOTAL:</div>");
            sb.Append($"<div class='flex-child-right-l-75'>{order.TotalAmount.Value.ToString("0.00")}</div>");
            sb.Append($"</div>");
            sb.Append($"</body>");
            sb.Append($"</html>");

            return sb.ToString();
        }

        private string GetRGAReason(int? rGAReason)
        {
            var result = string.Empty;

            switch (rGAReason)
            {
                case 0:
                    result =string.Empty; break;
                case 1:
                    result = "Damaged"; break;
                case 2:
                    result = "Agent Sold Wrong"; break;
                case 3:
                    result = "Car Totaled"; break;
                case 4:
                    result = "Defective"; break;
                case 5:
                    result = "Customer Do Not Need"; break;
                case 6:
                    result = "Price and Billing Adjustment"; break;
                case 7:
                    result = "Manager Approval"; break;
                case 8:
                    result = "Price Too High"; break;
                case 9:
                    result = "Got Somewhere Else"; break;
                case 10:
                    result = "Customer Ordered Wrong"; break;
            }

            return result;
        }

        private async Task<string> GenerateStatementAttachment(Customer customer, StatementReportDTO statement, DateTime reportDate, DateTime dueDate)
        {
            var folderName = Path.Combine("Assets", "img");
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            // Prepare Variables
            var companyName = customer.State == "CA" ? "Perfect Fit West LLC" : "PartsCo Inc.";
            var address1 = customer.State == "CA" ? "3383 Olive Ave" : "5151 W Oquendo Rd";
            var address2 = customer.State == "CA" ? "Signal Hill, CA 90755" : "Las Vegas, NV 89118";
            var phoneNumber = customer.State == "CA" ? "+1 3109564667" : "702-998-8888";
            var webSite = customer.State == "CA" ? "sales@perfectfitwest.com" : "PartsCoInc.com";
            var pngBinaryData = System.IO.File.ReadAllBytes(customer.State == "CA" ? $"{logoPath}\\pfitwest.png" : $"{logoPath}\\partsco.jpg");
            var ImgDataURI = @"data:image/png;base64," + Convert.ToBase64String(pngBinaryData);

            var sb = new StringBuilder();
            sb.Append(@"<html>");
            sb.Append(@"<head>");

            sb.Append(@"<style>body {font-family: Tahoma; font-size: medium}</style>");
            //sb.Append(@"<style>#section-print { display: block; left: 0; top: 0; }</style>");
            //sb.Append(@"<style>#page { size: auto; size: 8.5in 11in; margin-top: 20px; page-break-after: always; }</style>");
            //sb.Append(@"<style>#page-container { position: relative; min-height: 97vh; }</style>");
            //sb.Append(@"<style>#content-wrap { padding-bottom: 2rem; }</style>");
            //sb.Append(@"<style>#footer { text-align: right; position: absolute; bottom: 0; width: 100%; height: 2rem; }</style>");

            //sb.Append(@"<style>.flex-parent { display: flex; width: 100%; }</style>");
            //sb.Append(@"<style>.flex-child-left { flex: 1;text-align: left; }</style>");
            //sb.Append(@"<style>.flex-child-center { flex: 1; text-align: center; }</style>");
            //sb.Append(@"<style>.flex-child-right { flex: 1; text-align: right; }</style>");
            //sb.Append(@"<style>.flex-child-left-250 { flex: none; text-align: left; width: 250px; }</style>");
            //sb.Append(@"<style>.flex-child-left-200 { flex: none; text-align: left; width: 200px; }</style>");
            //sb.Append(@"<style>.flex-child-center-100 { flex: none; text-align: center; width: 100px; }</style>");
            //sb.Append(@"<style>.flex-child-right-100 { flex: none; text-align: right; width: 100px; }</style>");
            //sb.Append(@"<style>.flex-parent-gray { display: flex; width: 100%; }</style>");

            sb.Append(@"</head>");

            sb.Append(@"<body id='section-print'>");
            sb.Append(@"<div id='page'>");
            sb.Append(@"<div id='page-container'>");
            sb.Append(@"<div id='content-wrap'>");
            sb.Append(@"<div style='padding-bottom: 100px; '>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(@"<br>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(@"<div class='flex-child-left'>");
            sb.Append(string.Format("<div class='flex-child-left' style='padding-left: 20px;'>{0}</div>", companyName));
            sb.Append(string.Format("<div class='flex-child-left' style='padding-left: 20px;'>{0}</div>", address1));
            sb.Append(string.Format("<div class='flex-child-left' style='padding-left: 20px;'>{0}</div>", address2));
            sb.Append(string.Format("<div class='flex-child-left' style='padding-left: 20px;'>{0}</div>", phoneNumber));
            sb.Append(string.Format("<div class='flex-child-left' style='padding-left: 20px;'>{0}</div>", webSite));
            sb.Append(@"</div>");
            sb.Append(@"<div class='flex-child-right'>");
            sb.Append(string.Format("<img style='width: 300px; height: 75px;' src='{0}'>", ImgDataURI));
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(@"<div class='flex-child-left' style='font-size: xx-large; font-weight: normal; padding-left: 20px;'>Statement</div>");
            sb.Append(@"<div class='flex-child-right'>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(@"<div class='flex-child-right'>Payment Due on:&nbsp;</div>");
            sb.Append(string.Format("<div class='flex-child-left'>{0}</div>", dueDate.ToString("MM/dd/yyyy")));
            sb.Append(@"</div>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(@"<div class='flex-child-right'>Term:&nbsp;</div>");
            sb.Append(string.Format("<div class='flex-child-left'>{0}</div>", statement.PaymentTermName));
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(string.Format("<div class='flex-child-left' style='padding-left: 20px;'>TO: {0}</div>", statement.CustomerName));
            sb.Append(@"<div class='flex-child-right'>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(@"<div class='flex-child-right'>ACCOUNT NUMBER:&nbsp;</div>");
            sb.Append(string.Format("<div class='flex-child-left'>{0}</div>", statement.AccountNumber));
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(string.Format("<div class='flex-child-left' style='padding-left: 45px;'>{0}</div>", statement.AddressLine1));
            sb.Append(@"<div class='flex-child-right'>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(@"<div class='flex-child-right'>DATE:&nbsp;</div>");
            sb.Append(string.Format("<div class='flex-child-left'>{0}</div>", reportDate.ToString("MM/dd/yyyy")));
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(string.Format("<div class='flex-child-left' style='padding-left: 45px;'>{0}, {1} {2}</div>", statement.City, statement.State, statement.ZipCode));
            sb.Append(@"<div class='flex-child-right'>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(@"<div class='flex-child-right'>TOTAL DUE:&nbsp;</div>");
            sb.Append(string.Format("<div class='flex-child-left'> ${0:0.00}</div>", statement.TotalDue));
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(string.Format("<div class='flex-child-left' style='padding-left: 45px;'>{0}</div>", statement.PhoneNumber));
            sb.Append(@"<div class='flex-child-right'>");
            sb.Append(@"<div class='flex-parent'>");
            sb.Append(@"<div class='flex-child-right'>Amount ENCLOSED </div>");
            sb.Append(@"<div class='flex-child-left'></div>");
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"<br>");
            sb.Append(@"<div class='flex-parent-gray'>");
            sb.Append(@"<div class='flex-child-center-100'>DATE</div>");
            sb.Append(@"<div class='flex-child-left-250'>REFERENCE</div>");
            sb.Append(@"<div class='flex-child-left-200'>PURCHASE ORDER NUMBER</div>");
            sb.Append(@"<div class='flex-child-right-100'>AMOUNT</div>");
            sb.Append(@"<div class='flex-child-right-100'>BALANCE</div>");
            sb.Append(@"</div>");

            foreach (var order in statement.Orders)
            {
                sb.Append(@"<div class='flex-parent'>");
                sb.Append(string.Format("<div class='flex-child-center-100'>{0}</div>", order.OrderDate.ToString("MM/dd/yyyy")));
                sb.Append(string.Format("<div class='flex-child-left-250'>{0}: {1}, Invoice: {2}</div>", order.OrderType, order.OrderNumber, order.InvoiceNumber));
                sb.Append(string.Format("<div class='flex-child-left-200'>{0}</div>", order.PurchaseOrderNumber));
                sb.Append(string.Format("<div class='flex-child-right-100'>${0:0.00}</div>", order.TotalAmount.Value));
                sb.Append(string.Format("<div class='flex-child-right-100'>${0:0.00}</div>", order.Balance.Value));
                sb.Append(@"</div>");
            }

            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(string.Format(@"<footer id='footer'>Amount Due: ${0:0.00}</footer>", statement.TotalDue));
            sb.Append(@"</div>");
            sb.Append(@"</div>");
            sb.Append(@"</body>");
            sb.Append(@"</html>");

            return sb.ToString();
        }
    }
}
