import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { WarehouseLocation, WarehouseLocationWithStockDTO } from "./interfaces/models";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class WarehouseLocationService {
    url = "WarehouseLocation"
    constructor(private http: HttpClient) {}

    public getWarehouseLocations() : Observable<WarehouseLocation[]> {
        return this.http.get<WarehouseLocation[]>(`${environment.apiUrl}/${this.url}/GetWarehouseLocations`);
    }

    public getWarehouseLocationById(id: number) : Observable<WarehouseLocation> {
        return this.http.get<WarehouseLocation>(`${environment.apiUrl}/${this.url}/GetWarehouseLocationById?warehouseLocationId=${id}`);
    }

    public getWarehouseLocationByLocation(state: number, location: string) : Observable<WarehouseLocation> {
        return this.http.get<WarehouseLocation>(`${environment.apiUrl}/${this.url}/GetWarehouseLocationByLocation?state=${state}&location=${location}`);
    }

    public getWarehouseLocationByLocationWithStocks(state: number, location: string) : Observable<WarehouseLocationWithStockDTO> {
        return this.http.get<WarehouseLocationWithStockDTO>(`${environment.apiUrl}/${this.url}/GetWarehouseLocationByLocationWithStocks?state=${state}&location=${location}`);
    }

    public createWarehouseLocation(warehouseLocation: WarehouseLocation) : Observable<WarehouseLocation[]> {
        return this.http.post<WarehouseLocation[]>(`${environment.apiUrl}/${this.url}/CreateWarehouseLocation`, warehouseLocation);
    }

    public createWarehouseLocationWithStock(warehouseLocation: WarehouseLocation) : Observable<WarehouseLocation[]> {
        return this.http.post<WarehouseLocation[]>(`${environment.apiUrl}/${this.url}/CreateWarehouseLocationWithStock`, warehouseLocation);
    }

    public updateWarehouseLocation(warehouseLocation: WarehouseLocation) : Observable<WarehouseLocation[]> {
        return this.http.put<WarehouseLocation[]>(`${environment.apiUrl}/${this.url}/UpdateWarehouseLocation`, warehouseLocation);
    }

    public updateWarehouseLocationWithStock(warehouseLocation: WarehouseLocation) : Observable<WarehouseLocation[]> {
        return this.http.put<WarehouseLocation[]>(`${environment.apiUrl}/${this.url}/UpdateWarehouseLocationWithStock`, warehouseLocation);
    }

    public deleteWarehouseLocation(warehouseLocations: WarehouseLocation[]) : Observable<WarehouseLocation[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: warehouseLocations.map(a => a.id),
          };
          
        return this.http.delete<WarehouseLocation[]>(`${environment.apiUrl}/${this.url}/DeleteWarehouseLocation`, options);
    }
}