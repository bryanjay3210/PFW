import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { DailyVendorSalesSummaryDTO, PurchaseOrder, PurchaseOrderDetail, PurchaseOrderPaginatedListDTO } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class PurchaseOrderService {
    url = "PurchaseOrder"
    constructor(private http: HttpClient) {}

    public getPurchaseOrders() : Observable<PurchaseOrder[]> {
        return this.http.get<PurchaseOrder[]>(`${environment.apiUrl}/${this.url}/GetPurchaseOrders`);
    }

    public getPurchaseOrdersPaginated(pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<PurchaseOrderPaginatedListDTO> {
        return this.http.get<PurchaseOrderPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetPurchaseOrdersPaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getPurchaseOrdersByDatePaginated(pageSize: number, pageIndex: number, fromDate: any, toDate: any) : Observable<PurchaseOrderPaginatedListDTO> {
        return this.http.get<PurchaseOrderPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetPurchaseOrdersByDatePaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&fromDate=${fromDate}&toDate=${toDate}`);
    }

    public getDailySalesSummary(currentDate: any) : Observable<DailyVendorSalesSummaryDTO> {
        return this.http.get<DailyVendorSalesSummaryDTO>(`${environment.apiUrl}/${this.url}/GetDailyVendorSalesSummary?currentDate=${currentDate}`);
    }
    public getDailySalesSummaryByDate(fromDate: any, toDate: any) : Observable<DailyVendorSalesSummaryDTO> {
        return this.http.get<DailyVendorSalesSummaryDTO>(`${environment.apiUrl}/${this.url}/GetDailyVendorSalesSummaryByDate?fromDate=${fromDate}&toDate=${toDate}`);
    }

    public createPurchaseOrder(purchaseOrder: PurchaseOrder) : Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/CreatePurchaseOrder`, purchaseOrder);
    }

    public updatePurchaseOrder(purchaseOrder: PurchaseOrder) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdatePurchaseOrder`, purchaseOrder);
    }

    public softDeletePurchaseOrderDetail(purchaseOrderDetail: PurchaseOrderDetail) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/SoftDeletePurchaseOrderDetail`, purchaseOrderDetail);
    }

    public deletePurchaseOrder(purchaseOrder: PurchaseOrder) : Observable<boolean> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: purchaseOrder,
          };
          
        return this.http.delete<boolean>(`${environment.apiUrl}/${this.url}/DeletePurchaseOrder`, options);
    }
}