import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { Vendor, VendorOrderDTO } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class VendorService {
    url = "Vendor"
    constructor(private http: HttpClient) {}

    public getVendors() : Observable<Vendor[]> {
        return this.http.get<Vendor[]>(`${environment.apiUrl}/${this.url}/GetVendors`);
    }

    public getVendorsByState(state: string) : Observable<Vendor[]> {
        return this.http.get<Vendor[]>(`${environment.apiUrl}/${this.url}/GetVendorsByState?state=${state}`);
    }

    public getVendorOrdersByVendorCode(vendorCode: string) : Observable<VendorOrderDTO[]> {
        return this.http.get<VendorOrderDTO[]>(`${environment.apiUrl}/${this.url}/GetVendorOrdersByVendorCode?vendorCode=${vendorCode}`);
    }

    public createVendor(vendor: Vendor) : Observable<Vendor[]> {
        return this.http.post<Vendor[]>(`${environment.apiUrl}/${this.url}/CreateVendor`, vendor);
    }

    public updateVendor(vendor: Vendor) : Observable<Vendor[]> {
        return this.http.put<Vendor[]>(`${environment.apiUrl}/${this.url}/UpdateVendor`, vendor);
    }

    public deleteVendor(vendors: Vendor[]) : Observable<Vendor[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: vendors.map(a => a.id),
          };
          
        return this.http.delete<Vendor[]>(`${environment.apiUrl}/${this.url}/DeleteVendor`, options);
    }
}