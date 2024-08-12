import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { WarehouseStock, WarehouseStockDTO } from "./interfaces/models";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class WarehouseStockService {
    url = "WarehouseStock"
    constructor(private http: HttpClient) {}

    public getWarehouseStocks() : Observable<WarehouseStock[]> {
        return this.http.get<WarehouseStock[]>(`${environment.apiUrl}/${this.url}/GetWarehouseStocks`);
    }

    public getWarehouseStockById(id: number) : Observable<WarehouseStock> {
        return this.http.get<WarehouseStock>(`${environment.apiUrl}/${this.url}/GetWarehouseStockById?warehouseStockId=${id}`);
    }

    public createWarehouseStock(warehouseStock: WarehouseStock) : Observable<WarehouseStock[]> {
        return this.http.post<WarehouseStock[]>(`${environment.apiUrl}/${this.url}/CreateWarehouseStock`, warehouseStock);
    }

    public updateWarehouseStock(warehouseStock: WarehouseStock) : Observable<WarehouseStock[]> {
        return this.http.put<WarehouseStock[]>(`${environment.apiUrl}/${this.url}/UpdateWarehouseStock`, warehouseStock);
    }

    public updateWarehouseStocks(warehouseStocks: WarehouseStockDTO[]) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdateWarehouseStocks`, warehouseStocks);
    }

    public updateCycleCount(warehouseStocks: WarehouseStockDTO[]) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdateCycleCount`, warehouseStocks);
    }

    public transferWarehouseStocks(warehouseStocks: WarehouseStockDTO[]) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/TransferWarehouseStocks`, warehouseStocks);
    }
    // public updateWarehouseStocks(warehouseStocks: WarehouseStockDTO[]) : Observable<boolean> {
    //     const options = {
    //         headers: new HttpHeaders({
    //           'Content-Type': 'application/json',
    //         }),
    //         body: warehouseStocks,
    //       };

    //     return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdateWarehouseStocks`, options);
    // }

    public deleteWarehouseStock(warehouseStocks: WarehouseStock[]) : Observable<WarehouseStock[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: warehouseStocks.map(a => a.id),
          };
          
        return this.http.delete<WarehouseStock[]>(`${environment.apiUrl}/${this.url}/DeleteWarehouseStock`, options);
    }
}