import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { Zone } from "./interfaces/models";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class ZoneService {
    url = "Zone"
    constructor(private http: HttpClient) {}

    public getZones() : Observable<Zone[]> {
        return this.http.get<Zone[]>(`${environment.apiUrl}/${this.url}/GetZones`);
    }

    public getZoneById(id: number) : Observable<Zone> {
        return this.http.get<Zone>(`${environment.apiUrl}/${this.url}/GetZoneById?zoneId=${id}`);
    }

    public getZoneByZipCode(zipCode: string) : Observable<Zone> {
        return this.http.get<Zone>(`${environment.apiUrl}/${this.url}/GetZoneByZipCode?zipCode=${zipCode}`);
    }

    public createZone(zone: Zone) : Observable<Zone[]> {
        return this.http.post<Zone[]>(`${environment.apiUrl}/${this.url}/CreateZone`, zone);
    }

    public updateZone(zone: Zone) : Observable<Zone[]> {
        return this.http.put<Zone[]>(`${environment.apiUrl}/${this.url}/UpdateZone`, zone);
    }

    public deleteZone(zones: Zone[]) : Observable<Zone[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: zones.map(a => a.id),
          };
          
        return this.http.delete<Zone[]>(`${environment.apiUrl}/${this.url}/DeleteZone`, options);
    }
}