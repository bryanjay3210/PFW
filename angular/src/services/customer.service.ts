import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { Customer, CustomerDTO, CustomerDTOPaginatedListDTO, CustomerEmailDTO, CustomerPaginatedListDTO, Location } from "./interfaces/models";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class CustomerService {
    url = "Customer"
    
    constructor(private http: HttpClient) {}

    public getCustomers() : Observable<Customer[]> {
        return this.http.get<Customer[]>(`${environment.apiUrl}/${this.url}/GetCustomers`);
    }

    public getCustomersPaginated(pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<CustomerPaginatedListDTO> {
        return this.http.get<CustomerPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetCustomersPaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getCustomersList() : Observable<CustomerDTO[]> {
        return this.http.get<CustomerDTO[]>(`${environment.apiUrl}/${this.url}/GetCustomersList`);
    }

    public getCustomersListPaginated(pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<CustomerDTOPaginatedListDTO> {
        return this.http.get<CustomerDTOPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetCustomersListPaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getReportCustomersListPaginated(pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string, searchPaymentTermId?: number, searchState?: string) : Observable<CustomerDTOPaginatedListDTO> {
        return this.http.get<CustomerDTOPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetReportCustomersListPaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}&searchPaymentTermId=${searchPaymentTermId}&searchState=${searchState}`);
    }

    public getCustomerById(customerId: number) : Observable<CustomerDTO> {
        return this.http.get<CustomerDTO>(`${environment.apiUrl}/${this.url}/GetCustomerById?customerId=${customerId}`);
    }

    public getCustomerEmailsById(customerId: number) : Observable<CustomerEmailDTO[]> {
        return this.http.get<CustomerEmailDTO[]>(`${environment.apiUrl}/${this.url}/GetCustomerEmailsById?customerId=${customerId}`);
    }

    public getCustomerByAccountNumber(accountNumber: number) : Observable<CustomerDTO> {
        return this.http.get<CustomerDTO>(`${environment.apiUrl}/${this.url}/GetCustomerByAccountNumber?accountNumber=${accountNumber}`);
    }

    public createCustomer(customer: Customer) : Observable<Customer> {
        return this.http.post<Customer>(`${environment.apiUrl}/${this.url}/CreateCustomer`, customer);
    }

    public updateCustomer(customer: Customer) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdateCustomer`, customer);
    }

    public deleteCustomer(customers: Customer[]) : Observable<Customer[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: customers.map(a => a.id),
          };
          
        return this.http.delete<Customer[]>(`${environment.apiUrl}/${this.url}/DeleteCustomer`, options);
    }
}