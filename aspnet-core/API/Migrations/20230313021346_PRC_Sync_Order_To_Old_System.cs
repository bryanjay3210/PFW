using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class PRC_Sync_Order_To_Old_System : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = 
			@"-- =============================================
				-- Author:		Noel Jonathan Reambillo	
				-- Create date: 03/10/2023
				-- Description:	Synch Order from the new System into the Old POS System By OrderId
				-- exec PRC_Sync_Order_To_Old_System
				-- =============================================
				CREATE   PROCEDURE [dbo].[PRC_Sync_Order_To_Old_System]
					@OrderNumber INT
				AS
				BEGIN
					DECLARE @OrderId INT
					DECLARE @OldOrderId INT 
					DECLARE @OldSoNum INT
					DECLARE @OldInvNum  INT

					SET @OrderId = (SELECT Id FROM [PerfectFitWestDB].[dbo].[Orders] WHERE OrderNumber = @OrderNumber)
					INSERT INTO [IMS_WMS].[dbo].[TranHead]
					(
						 [Whouse]
						,[Area]
						,[Phone]
						,[Trandate]
						,[Debit]
						,[Credit]
						,[Qty]
						,[Amount]
						,[Category]
						,[Ponum]
						,[Disc]
						,[Tax]
						,[Freight]
						,[Status]
						,[Acctype]
						,[Orderno]
						,[Ronum]
						,[Sphone]
						,[Scompany]
						,[Saddress]
						,[Scontact]
						,[Scity]
						,[Sst]
						,[Szip]
						,[Srep]
						,[Dnotes]
						,[Mnemonic]
						,[Idno]
						,[Terms]
						,[Disrate]
						,[Taxrate]
						,[Discount]
						,[EMAIL_ADD]
						,[Amtpaid]
						,[Balance]
						,[L_payment]
						,[Auctionid]
						,[Fwd]
						,[Transid]
						,[Authnum]
						,[Avsaddress]
						,[Avszip]
						,[Billto]
						,[Avscvv2]
						,[Linknumber]
						,[SPHONE1]
						,[VRYSGN_SETTLE]
						,[HANDLING_TYPE]
						,[ORDER_TYPE]
						,[PRIORITY]
						,[WAVE_PLAN]
						,[SH_PARTIAL]
						,[SH_CLOSED]
						,[ship_number]
						,[HOLD_INVOICE]
						,[HOLD_VERIFY]
						,[SADDRESS2]
						,[ORDER_DATE]
						,[COST_A]
						,[COST_L]
					)
	
					SELECT 
						 CASE WHEN T1.WarehouseId=1 THEN 'CA'
						 ELSE 'OTHER'
						 END
						,'FIT'
						,CASE WHEN LEN(T1.[BillPhoneNumber]) = 10 THEN UPPER(left(RIGHT(T1.[BillPhoneNumber],7),3) + '-' + right(RIGHT(T1.[BillPhoneNumber],7),4))
						 ELSE UPPER(RIGHT(T1.[BillPhoneNumber], LEN(T1.[BillPhoneNumber]) - 3))
						 END
						,DATEADD(HOUR, -8, T1.OrderDate)
						,T1.TotalAmount
						,0
						,0
						,T1.TotalAmount
						,''
						,T1.PurchaseOrderNumber
						,0
						,T1.TotalTax
						,0
						,UPPER(ISNULL(T1.OrderStatusName,''))
						,''
						,0
						,'b2bsite'
						,UPPER(ISNULL(T1.ShipPhoneNumber,''))
						,UPPER(ISNULL(T1.ShipAddressName,''))
						,UPPER(ISNULL(T1.ShipAddress,''))
						,UPPER(ISNULL(T1.ShipContactName,''))
						,UPPER(ISNULL(T1.ShipCity,''))
						,UPPER(ISNULL(T1.ShipState,''))
						,UPPER(ISNULL(T1.ShipZipCode,''))
						,UPPER(ISNULL(T1.[User],''))
						,CASE WHEN T1.DeliveryRoute = 1 THEN UPPER(ISNULL(T1.OrderedBy,'') + ',' +  ISNULL(T1.OrderedByNotes,'') + ',      DATE: ' + convert(varchar, T1.DeliveryDate, 1)  +  ' AM')
						 ELSE UPPER(ISNULL(T1.OrderedBy,'') + ',' +  ISNULL(T1.OrderedByNotes,'') + ',      DATE: ' +   + convert(varchar, T1.DeliveryDate, 1)  +  ' PM')
						 END 
						,UPPER(ISNULL(T1.[User],''))
						,T1.AccountNumber
						,UPPER(ISNULL(T1.PaymentTermName,''))
						,0
						,T1.TaxRate
						,T1.Discount
						,UPPER(ISNULL(T1.OrderedByEmail,''))
						,0
						,T1.TotalAmount
						,0
						,T1.OrderNumber
						,1
						,''
						,''
						,''
						,''
						,''
						,''
						,0
						,CASE WHEN LEN(T1.[BillPhoneNumber]) = 10 THEN UPPER(left(RIGHT(T1.[BillPhoneNumber],7),3) + '-' + right(RIGHT(T1.[BillPhoneNumber],7),4))
						 ELSE UPPER(RIGHT(T1.[BillPhoneNumber], LEN(T1.[BillPhoneNumber]) - 3))
						 END
						,0
						,CASE WHEN T1.DeliveryType = 2 THEN 4
						 ELSE T1.DeliveryType
						 END
						,null
						,0
						,0
						,0
						,0
						,''
						,0
						,0
						,''
						,DATEADD(HOUR, -8, T1.OrderDate)
						,ISNULL(T1.[CurrentCost],0)
						,1
					FROM [PerfectFitWestDB].[dbo].[Orders] T1 
					WHERE OrderNumber = @OrderNumber

					SET @OldOrderId = (SELECT [_ID] FROM [IMS_WMS].[dbo].[TranHead] WHERE Auctionid = @OrderNumber)
					SET @OldSoNum = (SELECT [Sonum] FROM [IMS_WMS].[dbo].[TranHead] WHERE Auctionid = @OrderNumber)
					SET @OldInvNum = (SELECT [Invno] FROM [IMS_WMS].[dbo].[TranHead] WHERE Auctionid = @OrderNumber)

					INSERT INTO [IMS_WMS].[dbo].[Trans]
					(
						 [Trandate]
						,[Pnumber]
						,[Qty]
						,[Price1]
						,[Mnemonic]
						,[Area]
						,[Phone]
						,[Sonum]
						,[Pdesc]
						,[Cat]
						,[Plist]
						,[Oh_before]
						,[Status]
						,[Oh_after]
						,[Bin]
						,[Edit_date]
						,[Q_order]
						,[Notes]
						,[parts_size]
						,[Q_SHIPPED]
						,[Q_SCANNED]
						,[EXCLUDED]
						,[Q_RF_RETURN]
						,[Q_PICK]
						,[HOLD]
						,[WAVE_PLAN]
						,[AUCTIONID]
						,[item_catalog]
						,[Trackingno]
						,[Carrier]
						,[UnitCost]
						,[Vehicle]
					)
					SELECT
						 DATEADD(HOUR, -8, T1.CreatedDate)
						,T1.PartNumber
						,T1.OrderQuantity
						,T1.WholesalePrice
						,UPPER(T1.CreatedBy)
						,'CA'
						,CASE WHEN LEN(T2.[BillPhoneNumber]) = 10 THEN UPPER(left(RIGHT(T2.[BillPhoneNumber],7),3) + '-' + right(RIGHT(T2.[BillPhoneNumber],7),4))
						 ELSE UPPER(RIGHT(T2.[BillPhoneNumber], LEN(T2.[BillPhoneNumber]) - 3))
						 END
						,@OldSoNum
						,UPPER(RTRIM(LTRIM(CAST(T1.[YearFrom] as nvarchar(4))))) + ',' +  UPPER(RTRIM(LTRIM(CAST(T1.[YearTo] as nvarchar(4))))) + ',' +   UPPER(ISNULL(T1.PartDescription,''))+ ',' +  UPPER(ISNULL(T1.VendorCode,''))--[Pdesc]
						,T1.CategoryId
						,T1.ListPrice
						,0
						,UPPER(ISNULL(T2.OrderStatusName,''))
						,0
						,'NOELTOADD'
						, DATEADD(HOUR, -8, T1.CreatedDate)
						,T1.OrderQuantity
						,CASE WHEN ISNULL(T1.[VendorCode],'') <> '' AND ISNULL(T1.VendorPartnumber,'') <>'' THEN UPPER(ISNULL(T1.[VendorCode],'') + '|' +  ISNULL(T1.VendorPartnumber,'') + '|' + CAST(ISNULL(T1.VendorPrice,0) as nvarchar(10)))
						 ELSE ''
						 END
						,CASE WHEN T1.PartSize=3 THEN 2
						 ELSE T1.PartSize
						 END
						,0
						,0
						,0
						,0
						,0
						,0
						,0
						,T2.OrderNumber
						,0
						,''
						,''
						,ISNULL(T1.[UnitCost],0)
						,''
					FROM [PerfectFitWestDB].[dbo].[OrderDetails] T1 
					LEFT JOIN [PerfectFitWestDB].[dbo].[Orders] T2 ON T1.OrderId= T2.Id
					WHERE T1.OrderId = @OrderID

					UPDATE [IMS_WMS].[dbo].[TranHead]
					SET [SINGLE_INVOICE] = (SELECT sum(OrderQuantity) FROM [PerfectFitWestDB].[dbo].[OrderDetails] WHERE OrderId= @OrderID)
					WHERE Auctionid = @OrderNumber
		   
					UPDATE [IMS_WMS].[dbo].[TranHead]
					SET [Invno] = @OldSoNum
					WHERE Auctionid = @OrderNumber

					INSERT INTO [IMS_WMS].[dbo].TranheadDelDate
					(
						 [SONUM]
						,[DelDate]
						,[AMPM]
					)
					SELECT 
						@OldSoNum
						,DATEADD(HOUR, -8, T1.DeliveryDate)
						,CASE WHEN T1.DeliveryRoute = 1 THEN 'AM'
						ELSE 'PM'
						END 

					FROM [PerfectFitWestDB].[dbo].[Orders] T1 
					WHERE OrderNumber = @OrderNumber

					UPDATE [PerfectFitWestDB].[dbo].[Orders]
					SET [InvoiceNumber] =@OldSoNum
					WHERE [id]=@OrderID

					UPDATE 
					[PerfectFitWestDB].[dbo].[OrderDetails]
					SET [SalesOrderNumber] = @OldSoNum
					WHERE OrderId = @OrderID

				END
				GO";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
