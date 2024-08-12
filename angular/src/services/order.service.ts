import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { CreateOrderResult, CustomerSalesAmountDTO, CustomerSalesFilterDTO, DailySalesSummaryDTO, DeliverySummaryDTO, Order, OrderPaginatedListDTO, OverpaymentParameterDTO, TotalSalesDTO } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class OrderService {
    url = "Order"
    constructor(private http: HttpClient) {}

    public getOrders() : Observable<Order[]> {
        return this.http.get<Order[]>(`${environment.apiUrl}/${this.url}/GetOrders`);
    }

    public getOrderByOrderNumber(orderNumber: number) : Observable<Order> {
        return this.http.get<Order>(`${environment.apiUrl}/${this.url}/GetOrderByOrderNumber?orderNumber=${orderNumber}`);
    }

    public getOrdersByCustomerId(customerId: number) : Observable<Order[]> {
        return this.http.get<Order[]>(`${environment.apiUrl}/${this.url}/GetOrdersByCustomerId?customerId=${customerId}`);
    }

    public getInvoicesByCustomerId(customerId: number) : Observable<Order[]> {
        return this.http.get<Order[]>(`${environment.apiUrl}/${this.url}/GetInvoicesByCustomerId?customerId=${customerId}`);
    }

    public getInvoicesByCustomerIds(customerIds: number[]) : Observable<Order[]> {
        return this.http.put<Order[]>(`${environment.apiUrl}/${this.url}/GetInvoicesByCustomerIds`, customerIds);
    }

    public getCreditMemoByInvoiceNumber(invoiceNumber: number) : Observable<Order[]> {
        return this.http.get<Order[]>(`${environment.apiUrl}/${this.url}/GetCreditMemoByInvoiceNumber?invoiceNumber=${invoiceNumber}`);
    }

    public getDiscountsByInvoiceNumber(invoiceNumber: number, partNumbers: string[]) : Observable<Order[]> {
        return this.http.put<Order[]>(`${environment.apiUrl}/${this.url}/GetDiscountsByInvoiceNumber?invoiceNumber=${invoiceNumber}`, partNumbers);
    }

    public getCreditMemoByCustomerId(customerId: number) : Observable<Order[]> {
        return this.http.get<Order[]>(`${environment.apiUrl}/${this.url}/GetCreditMemoByCustomerId?customerId=${customerId}`);
    }

    public getCreditMemoByCustomerIds(customerIds: number[]) : Observable<Order[]> {
        return this.http.put<Order[]>(`${environment.apiUrl}/${this.url}/GetCreditMemoByCustomerIds`, customerIds);
    }

    public getQuotes() : Observable<Order[]> {
        return this.http.get<Order[]>(`${environment.apiUrl}/${this.url}/GetQuotes`);
    }

    public getOrdersPaginated(searchType: number, pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string, paymentTerm?: number) : Observable<OrderPaginatedListDTO> {
        return this.http.get<OrderPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetOrdersPaginated?searchType=${searchType}&pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}&paymentTerm=${paymentTerm}`);
    }

    // public getOrdersByDatePaginated(pageSize: number, pageIndex: number, fromDate: any, toDate: any): Observable<OrderPaginatedListDTO> {
    //   return this.http.get<OrderPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetOrdersByDatePaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&fromDate=${fromDate}&toDate=${toDate}`);
    // }

    public getOrdersByDatePaginated(searchType: number, pageSize: number, pageIndex: number, fromDate: any, toDate: any, search?: string, paymentTerm?: number) : Observable<OrderPaginatedListDTO> {
       return this.http.get<OrderPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetOrdersByDatePaginated?searchType=${searchType}&pageSize=${pageSize}&pageIndex=${pageIndex}&fromDate=${fromDate}&toDate=${toDate}&search=${search}&paymentTerm=${paymentTerm}`);
    }

    public getDailySalesSummary(currentDate: any) : Observable<DailySalesSummaryDTO> {
        return this.http.get<DailySalesSummaryDTO>(`${environment.apiUrl}/${this.url}/GetDailySalesSummary?currentDate=${currentDate}`);
    }

    public getCustomerSales(filter: CustomerSalesFilterDTO) : Observable<CustomerSalesAmountDTO[]> {
        return this.http.put<CustomerSalesAmountDTO[]>(`${environment.apiUrl}/${this.url}/GetCustomerSales`, filter);
    }

    public getDailySalesSummaryByDate(fromDate: any, toDate: any) : Observable<DailySalesSummaryDTO> {
        return this.http.get<DailySalesSummaryDTO>(`${environment.apiUrl}/${this.url}/GetDailySalesSummaryByDate?fromDate=${fromDate}&toDate=${toDate}`);
    }

    public getCustomerOrdersPaginated(customerId: number, pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<OrderPaginatedListDTO> {
        return this.http.get<OrderPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetCustomerOrdersPaginated?customerId=${customerId}&pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getQuotesPaginated(pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<OrderPaginatedListDTO> {
        return this.http.get<OrderPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetQuotesPaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getWebOrdersPaginated(pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<OrderPaginatedListDTO> {
        return this.http.get<OrderPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetWebOrdersPaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getRGAOrdersPaginated(pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<OrderPaginatedListDTO> {
        return this.http.get<OrderPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetRGAOrdersPaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getReportOrdersListPaginated(deliveryDate: Date, pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string, state?: string, deliveryRoute?: number) : Observable<OrderPaginatedListDTO> {
        return this.http.get<OrderPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetReportOrdersListPaginated?deliveryDate=${deliveryDate}&pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}&state=${state}&deliveryRoute=${deliveryRoute}`);
    }

    public getDailyTotalSalesSummaryCA() : Observable<TotalSalesDTO[]> {
        return this.http.get<TotalSalesDTO[]>(`${environment.apiUrl}/${this.url}/GetDailyTotalSalesSummaryCA`);
    }

    public getDailyTotalSalesSummaryNV() : Observable<TotalSalesDTO[]> {
        return this.http.get<TotalSalesDTO[]>(`${environment.apiUrl}/${this.url}/GetDailyTotalSalesSummaryNV`);
    }

    public getDeliverySummary(currentDate: any) : Observable<DeliverySummaryDTO> {
        return this.http.get<DeliverySummaryDTO>(`${environment.apiUrl}/${this.url}/GetDeliverySummary?currentDate=${currentDate}`);
    }

    public voidOrder(order: Order) : Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/VoidOrder`, order);
    }

    public deleteRGAOrder(order: Order) : Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/DeleteRGAOrder`, order);
    }

    //ConvertQuoteToOrder
    public convertQuoteToOrder(order: Order) : Observable<CreateOrderResult> {
        return this.http.put<CreateOrderResult>(`${environment.apiUrl}/${this.url}/ConvertQuoteToOrder`, order);
    }

    public createOrder(order: Order) : Observable<CreateOrderResult> {
        return this.http.post<CreateOrderResult>(`${environment.apiUrl}/${this.url}/CreateOrder`, order);
    }

    public createCreditMemo(order: Order) : Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/CreateCreditMemo`, order);
    }

    public createRGA(order: Order) : Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/CreateRGA`, order);
    }

    public createDiscount(order: Order) : Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/CreateDiscount`, order);
    }

    public createOverpayment(param: OverpaymentParameterDTO) : Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/CreateOverpayment`, param);
    }

    public updateOrder(order: Order) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdateOrder`, order);
    }

    public updateOrderInspectedCode(order: Order) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdateOrderInspectedCode`, order);
    }

    public updateOrderStatus(order: Order) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdateOrderStatus`, order);
    }

    public updateOrderSummary(order: Order) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdateOrderSummary`, order);
    }

    public deleteOrderDetail(orderDetailId: number) : Observable<boolean> {
        return this.http.delete<boolean>(`${environment.apiUrl}/${this.url}/DeleteOrderDetail?orderDetailId=${orderDetailId}`);
    }

    public deleteOrder(contacts: Order[]) : Observable<Order[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: contacts.map(a => a.id),
          };
          
        return this.http.delete<Order[]>(`${environment.apiUrl}/${this.url}/DeleteOrder`, options);
    }
}
