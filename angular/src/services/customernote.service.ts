import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { CustomerNote } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class CustomerNoteService {
    url = "CustomerNote"
    constructor(private http: HttpClient) {}

    public getCustomerNotes() : Observable<CustomerNote[]> {
        return this.http.get<CustomerNote[]>(`${environment.apiUrl}/${this.url}/GetCustomerNotes`);
    }

    public getCustomerNotesByCustomerId(customerId: number) : Observable<CustomerNote[]> {
        return this.http.get<CustomerNote[]>(`${environment.apiUrl}/${this.url}/GetCustomerNotesByCustomerId?customerId=${customerId}`);
    }

    public createCustomerNote(contact: CustomerNote) : Observable<CustomerNote[]> {
        return this.http.post<CustomerNote[]>(`${environment.apiUrl}/${this.url}/CreateCustomerNote`, contact);
    }

    public updateCustomerNote(contact: CustomerNote) : Observable<CustomerNote[]> {
        return this.http.put<CustomerNote[]>(`${environment.apiUrl}/${this.url}/UpdateCustomerNote`, contact);
    }

    public deleteCustomerNote(contacts: CustomerNote[]) : Observable<CustomerNote[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: contacts.map(a => a.id),
          };
          
        return this.http.delete<CustomerNote[]>(`${environment.apiUrl}/${this.url}/DeleteCustomerNote`, options);
    }
}