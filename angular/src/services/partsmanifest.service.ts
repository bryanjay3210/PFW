import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { PartsManifest, PartsManifestPaginatedListDTO } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class PartsManifestService {
    url = "PartsManifest"
    constructor(private http: HttpClient) {}

    public getPartsManifests() : Observable<PartsManifest[]> {
        return this.http.get<PartsManifest[]>(`${environment.apiUrl}/${this.url}/GetPartsManifests`);
    }

    public getPartsManifestsPaginated(pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<PartsManifestPaginatedListDTO> {
        return this.http.get<PartsManifestPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetPartsManifestsPaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getPartsManifestsByDatePaginated(pageSize: number, pageIndex: number, fromDate: any, toDate: any) : Observable<PartsManifestPaginatedListDTO> {
        return this.http.get<PartsManifestPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetPartsManifestsByDatePaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&fromDate=${fromDate}&toDate=${toDate}`);
    }

    public createPartsManifest(driverLog: PartsManifest) : Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/CreatePartsManifest`, driverLog);
    }

    public updatePartsManifest(driverLog: PartsManifest) : Observable<PartsManifest[]> {
        return this.http.put<PartsManifest[]>(`${environment.apiUrl}/${this.url}/UpdatePartsManifest`, driverLog);
    }

    public deletePartsManifest(driverLogs: PartsManifest[]) : Observable<PartsManifest[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: driverLogs.map(a => a.id),
          };
          
        return this.http.delete<PartsManifest[]>(`${environment.apiUrl}/${this.url}/DeletePartsManifest`, options);
    }
}