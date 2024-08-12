import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { OrderNote } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class OrderNoteService {
    url = "OrderNote"
    constructor(private http: HttpClient) {}

    public getOrderNotes() : Observable<OrderNote[]> {
        return this.http.get<OrderNote[]>(`${environment.apiUrl}/${this.url}/GetOrderNotes`);
    }

    public getOrderNotesByOrderId(orderId: number) : Observable<OrderNote[]> {
        return this.http.get<OrderNote[]>(`${environment.apiUrl}/${this.url}/GetOrderNotesByOrderId?orderId=${orderId}`);
    }

    public createOrderNote(contact: OrderNote) : Observable<OrderNote[]> {
        return this.http.post<OrderNote[]>(`${environment.apiUrl}/${this.url}/CreateOrderNote`, contact);
    }

    public updateOrderNote(contact: OrderNote) : Observable<OrderNote[]> {
        return this.http.put<OrderNote[]>(`${environment.apiUrl}/${this.url}/UpdateOrderNote`, contact);
    }

    public deleteOrderNote(contacts: OrderNote[]) : Observable<OrderNote[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: contacts.map(a => a.id),
          };
          
        return this.http.delete<OrderNote[]>(`${environment.apiUrl}/${this.url}/DeleteOrderNote`, options);
    }
}