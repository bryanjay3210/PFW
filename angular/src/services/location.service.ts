import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { Location, LocationDTO } from "./interfaces/models";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class LocationService {
    url = "Location"
    constructor(private http: HttpClient) {}

    public getLocations() : Observable<Location[]> {
        return this.http.get<Location[]>(`${environment.apiUrl}/${this.url}/GetLocations`);
    }

    public getLocationsByCustomerId(customerId: number) : Observable<Location[]> {
        return this.http.get<Location[]>(`${environment.apiUrl}/${this.url}/GetLocationsByCustomerId?customerId=${customerId}`);
    }

    public getLocationsList(customerId: number) : Observable<LocationDTO[]> {
        return this.http.get<LocationDTO[]>(`${environment.apiUrl}/${this.url}/GetLocationsList?customerId=${customerId}`);
    }

    public createLocation(location = {} as Location) : Observable<Location[]> {
        return this.http.post<Location[]>(`${environment.apiUrl}/${this.url}/CreateLocation`, location);
    }

    public updateLocation(location = {} as Location) : Observable<Location[]> {
        return this.http.put<Location[]>(`${environment.apiUrl}/${this.url}/UpdateLocation`, location);
    }

    public deleteLocation(locations = {} as Location[]) : Observable<Location[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: locations.map(a => a.id),
          };
          
        return this.http.delete<Location[]>(`${environment.apiUrl}/${this.url}/DeleteLocation`, options);
    }
}