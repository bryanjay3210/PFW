import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { Contact } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class ContactService {
    url = "Contact"
    constructor(private http: HttpClient) {}

    public getContacts() : Observable<Contact[]> {
        return this.http.get<Contact[]>(`${environment.apiUrl}/${this.url}/GetContacts`);
    }

    public getContactsByCustomerId(customerId: number) : Observable<Contact[]> {
        return this.http.get<Contact[]>(`${environment.apiUrl}/${this.url}/GetContactsByCustomerId?customerId=${customerId}`);
    }

    public createContact(contact: Contact) : Observable<Contact[]> {
        return this.http.post<Contact[]>(`${environment.apiUrl}/${this.url}/CreateContact`, contact);
    }

    public updateContact(contact: Contact) : Observable<Contact[]> {
        return this.http.put<Contact[]>(`${environment.apiUrl}/${this.url}/UpdateContact`, contact);
    }

    public deleteContact(contacts: Contact[]) : Observable<Contact[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: contacts.map(a => a.id),
          };
          
        return this.http.delete<Contact[]>(`${environment.apiUrl}/${this.url}/DeleteContact`, options);
    }
}