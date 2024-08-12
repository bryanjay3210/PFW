import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { CustomerCredit } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class CustomerCreditService {
    url = "CustomerCredit"
    constructor(private http: HttpClient) {}

    public getCustomerCreditById(customerCreditId: number) : Observable<CustomerCredit> {
        return this.http.get<CustomerCredit>(`${environment.apiUrl}/${this.url}/GetCustomerCreditById?customerCreditId=${customerCreditId}`);
    }

    public getCustomerCreditsByCustomerId(customerId: number) : Observable<CustomerCredit[]> {
        return this.http.get<CustomerCredit[]>(`${environment.apiUrl}/${this.url}/GetCustomerCreditsByCustomerId?customerId=${customerId}`);
    }

    public createCustomerCredit(customerCredit: CustomerCredit) : Observable<CustomerCredit> {
        return this.http.post<CustomerCredit>(`${environment.apiUrl}/${this.url}/CreateCustomerCredit`, customerCredit);
    }

    public updateCustomerCredit(customerCredit: CustomerCredit) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdateCustomerCredit`, customerCredit);
    }

    public deleteCustomerCredit(customerCredits: CustomerCredit[]) : Observable<CustomerCredit[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: customerCredits.map(a => a.id),
          };
          
        return this.http.delete<CustomerCredit[]>(`${environment.apiUrl}/${this.url}/DeleteCustomerCredit`, options);
    }
}