import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { PartsPicking, PartsPickingDetail, PartsPickingPaginatedListDTO, StockOrderDetailDTO } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class PartsPickingService {
    url = "PartsPicking"
    constructor(private http: HttpClient) {}

    public getPartsPickings() : Observable<PartsPicking[]> {
        return this.http.get<PartsPicking[]>(`${environment.apiUrl}/${this.url}/GetPartsPickings`);
    }

    public getPartsPickingsPaginated(pageSize: number, pageIndex: number, sortColumn?: string, sortOrder?: string, search?: string) : Observable<PartsPickingPaginatedListDTO> {
        return this.http.get<PartsPickingPaginatedListDTO>(`${environment.apiUrl}/${this.url}/GetPartsPickingsPaginated?pageSize=${pageSize}&pageIndex=${pageIndex}&sortColumn=${sortColumn}&sortOrder=${sortOrder}&search=${search}`);
    }

    public getStockOrderDetails(warehouseFilter: number) : Observable<StockOrderDetailDTO[]> {
        return this.http.get<StockOrderDetailDTO[]>(`${environment.apiUrl}/${this.url}/GetStockOrderDetails?warehouseFilter=${warehouseFilter}`);
    }


    public createPartsPicking(partsPicking: PartsPicking) : Observable<boolean> {
        return this.http.post<boolean>(`${environment.apiUrl}/${this.url}/CreatePartsPicking`, partsPicking);
    }

    public updatePartsPicking(partsPicking: PartsPicking) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdatePartsPicking`, partsPicking);
    }

    public softDeletePartsPickingDetail(partsPickingDetail: PartsPickingDetail) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/SoftDeletePartsPickingDetail`, partsPickingDetail);
    }

    public deletePartsPicking(partsPicking: PartsPicking) : Observable<boolean> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: partsPicking,
          };
          
        return this.http.delete<boolean>(`${environment.apiUrl}/${this.url}/DeletePartsPicking`, options);
    }
}