import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { StockSettings } from "./interfaces/models";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class StockSettingsService {
    url = "StockSettings"
    constructor(private http: HttpClient) {}

    public getStockSettings() : Observable<StockSettings> {
        return this.http.get<StockSettings>(`${environment.apiUrl}/${this.url}/GetStockSettings`);
    }

    public updateStockSettings(stockSettings: StockSettings) : Observable<boolean> {
        return this.http.put<boolean>(`${environment.apiUrl}/${this.url}/UpdateStockSettings`, stockSettings);
    }
}