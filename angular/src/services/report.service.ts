import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { AgingBalanceReportDTO, StatementReportDTO, StatementTotalReportDTO } from "./interfaces/models";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class ReportService {
    url = "Report"
    constructor(private http: HttpClient) {} 

    public getAgingBalanceReport(reportDate: any) : Observable<AgingBalanceReportDTO[]> {
        return this.http.get<AgingBalanceReportDTO[]>(`${environment.apiUrl}/${this.url}/GetAgingBalanceReport?reportDate=${reportDate}`);
    }

    public getStatementReport(reportDate: any, paymentTermId?: number, customerIds?: number[]) : Observable<StatementReportDTO[]> {
        return this.http.put<StatementReportDTO[]>(`${environment.apiUrl}/${this.url}/GetStatementReport?reportDate=${reportDate}&paymentTermId=${paymentTermId}`, customerIds);
    }

    public getStatementTotalReport(reportDate: any, paymentTermId?: number, customerIds?: number[]) : Observable<StatementTotalReportDTO[]> {
        return this.http.put<StatementTotalReportDTO[]>(`${environment.apiUrl}/${this.url}/GetStatementTotalReport?reportDate=${reportDate}&paymentTermId=${paymentTermId}`, customerIds);
    }

    public updatePrintedInvoice(orderIds?: number[]) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdatePrintedInvoice`, orderIds);
    }
}