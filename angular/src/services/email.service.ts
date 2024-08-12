import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { RegisterUserDTO } from "./interfaces/auth/registeruserdto.model";
import { DriverLog, StatementEmailParamDTO } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class EmailService {
    url = "Email"
    constructor(private http: HttpClient) {}

    public sendUserRegistrationEmail(userdto: RegisterUserDTO) : Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/SendUserRegistrationEmail`, userdto);
    }

    public sendDriverLogEmails(driverLog: DriverLog) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/SendDriverLogEmails`, driverLog);
    }

    public sendOrderEmail(orderId: number, contactName: string, email: string) : Observable<boolean> {
        return this.http.get<boolean>(`${environment.apiUrl}/${this.url}/SendOrderEmail?orderId=${orderId}&contactName=${contactName}&email=${email}`);
    }

    public sendStatementEmails(statementEmailParam: StatementEmailParamDTO) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/SendStatementEmails`, statementEmailParam);
    }

    // public sendStatementEmail(formData: any, customerId : number) : Observable<boolean> {
        // const options = {
        //     headers: new HttpHeaders({
        //       'Content-Type': 'application/json',
        //       'Data': formData
        //     }),
        //     body: contacts.map(a => a.id),
        //   };
        // return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/SendStatementEmail?customerId=${customerId}&data=${formData}`,formData);
    // }
}