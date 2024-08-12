import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { DriverLog, DriverLogPaginatedListDTO } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class DriverLogService {
    url = "DriverLog"
    constructor(private http: HttpClient) {}

    public getDriverLogs() : Observable<DriverLog[]> {
        return this.http.get<DriverLog[]>(`${environment.apiUrl}/${this.url}/GetDriverLogs`);
    }

    public getDriverLogsPaginated(pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<DriverLogPaginatedListDTO> {
        return this.http.get<DriverLogPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetDriverLogsPaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getDriverLogsByDatePaginated(pageSize: number, pageIndex: number, fromDate: any, toDate: any) : Observable<DriverLogPaginatedListDTO> {
        return this.http.get<DriverLogPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetDriverLogsByDatePaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&fromDate=${fromDate}&toDate=${toDate}`);
    }

    public createDriverLog(driverLog: DriverLog) : Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/CreateDriverLog`, driverLog);
    }

    public updateDriverLog(driverLog: DriverLog) : Observable<DriverLog[]> {
        return this.http.put<DriverLog[]>(`${environment.apiUrl}/${this.url}/UpdateDriverLog`, driverLog);
    }

    public deleteDriverLog(driverLogs: DriverLog[]) : Observable<DriverLog[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: driverLogs.map(a => a.id),
          };
          
        return this.http.delete<DriverLog[]>(`${environment.apiUrl}/${this.url}/DeleteDriverLog`, options);
    }
}