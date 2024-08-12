import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { DailyPaymentSummaryDTO, Payment, PaymentHistoryDTO, PaymentPaginatedListDTO } from "./interfaces/models";
import { map } from "rxjs";

@Injectable({
    providedIn: 'root'
})

export class PaymentService {
    url = "Payment"
    constructor(private http: HttpClient) {}

    public getPaymentById(paymentId: number) : Observable<Payment> {
        return this.http.get<Payment>(`${environment.apiUrl}/${this.url}/GetPaymentById?paymentId=${paymentId}`);
    }

    public getPaymentHistoryByOrderNumber(orderNumber: number) : Observable<PaymentHistoryDTO[]> {
        return this.http.get<PaymentHistoryDTO[]>(`${environment.apiUrl}/${this.url}/GetPaymentHistoryByOrderNumber?orderNumber=${orderNumber}`);
    }

    public getPaymentsPaginated(isIncludeInactive: boolean, pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<PaymentPaginatedListDTO> {
        return this.http.get<PaymentPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetPaymentsPaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getPaymentsByDatePaginated(pageSize: number, pageIndex: number, fromDate: any, toDate: any) : Observable<PaymentPaginatedListDTO> {
        return this.http.get<PaymentPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetPaymentsByDatePaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&fromDate=${fromDate}&toDate=${toDate}`);
    }

    public getDailyPaymentSummary(currentDate: any) : Observable<DailyPaymentSummaryDTO> {
        return this.http.get<DailyPaymentSummaryDTO>(`${environment.apiUrl}/${this.url}/GetDailyPaymentSummary?currentDate=${currentDate}`);
    }

    public getPaymentSummaryByDate(fromDate: any, toDate: any) : Observable<DailyPaymentSummaryDTO> {
        return this.http.get<DailyPaymentSummaryDTO>(`${environment.apiUrl}/${this.url}/GetPaymentSummaryByDate?fromDate=${fromDate}&toDate=${toDate}`);
    }

    public createPayment(payment: Payment) : Observable<Payment> {
        return this.http.post<Payment>(`${environment.apiUrl}/${this.url}/CreatePayment`, payment);
    }

    public createRefund(payment: Payment) : Observable<Payment> {
        return this.http.post<Payment>(`${environment.apiUrl}/${this.url}/CreateRefund`, payment);
    }

    public updatePayment(payment: Payment) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdatePayment`, payment);
    }

    public deletePayment(payments: Payment[]) : Observable<Payment[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: payments.map(a => a.id),
          };
          
        return this.http.delete<Payment[]>(`${environment.apiUrl}/${this.url}/DeletePayment`, options);
    }
}
