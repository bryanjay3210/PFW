import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { DriverLogDetail } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class DriverLogDetailService {
    url = "DriverLogDetail"
    constructor(private http: HttpClient) {}

    public getDriverLogDetails() : Observable<DriverLogDetail[]> {
        return this.http.get<DriverLogDetail[]>(`${environment.apiUrl}/${this.url}/GetDriverLogDetails`);
    }

    public getDriverLogDetailById(driverLogDetailId: string) : Observable<DriverLogDetail> {
        return this.http.get<DriverLogDetail>(`${environment.apiUrl}/${this.url}/GetDriverLogDetailById?driverLogDetailId=${driverLogDetailId}`);
    }

    public createDriverLogDetail(driverLogDetail: DriverLogDetail) : Observable<DriverLogDetail[]> {
        return this.http.post<DriverLogDetail[]>(`${environment.apiUrl}/${this.url}/CreateDriverLogDetail`, driverLogDetail);
    }

    public updateDriverLogDetail(driverLogDetail: DriverLogDetail) : Observable<DriverLogDetail[]> {
        return this.http.put<DriverLogDetail[]>(`${environment.apiUrl}/${this.url}/UpdateDriverLogDetail`, driverLogDetail);
    }

    public deleteDriverLogDetail(driverLogDetails: DriverLogDetail[]) : Observable<DriverLogDetail[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: driverLogDetails.map(a => a.id),
          };
          
        return this.http.delete<DriverLogDetail[]>(`${environment.apiUrl}/${this.url}/DeleteDriverLogDetail`, options);
    }
}