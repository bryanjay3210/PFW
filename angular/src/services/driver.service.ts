import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { Driver } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class DriverService {
    url = "Driver"
    constructor(private http: HttpClient) {}

    public getDrivers() : Observable<Driver[]> {
        return this.http.get<Driver[]>(`${environment.apiUrl}/${this.url}/GetDrivers`);
    }

    public getDriverByDriverNumber(driverNumber: string) : Observable<Driver> {
        return this.http.get<Driver>(`${environment.apiUrl}/${this.url}/GetDriverByDriverNumber?driverNumber=${driverNumber}`);
    }

    public createDriver(driverLog: Driver) : Observable<Driver[]> {
        return this.http.post<Driver[]>(`${environment.apiUrl}/${this.url}/CreateDriver`, driverLog);
    }

    public updateDriver(driverLog: Driver) : Observable<Driver[]> {
        return this.http.put<Driver[]>(`${environment.apiUrl}/${this.url}/UpdateDriver`, driverLog);
    }

    public deleteDriver(driverLogs: Driver[]) : Observable<Driver[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: driverLogs.map(a => a.id),
          };
          
        return this.http.delete<Driver[]>(`${environment.apiUrl}/${this.url}/DeleteDriver`, options);
    }
}