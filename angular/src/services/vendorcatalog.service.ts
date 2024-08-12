import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { ProductVendorCatalog, VendorCatalog } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class VendorCatalogService {
    url = "VendorCatalog"
    constructor(private http: HttpClient) {}

    public getVendorCatalogs() : Observable<VendorCatalog[]> {
        return this.http.get<VendorCatalog[]>(`${environment.apiUrl}/${this.url}/GetVendorCatalogs`);
    }

    public getVendorCatalogsByPartsLinkNumbers(partsLinkNumbers: string[]) : Observable<VendorCatalog[]> {
        return this.http.put<VendorCatalog[]>(`${environment.apiUrl}/${this.url}/GetVendorCatalogsByPartsLinkNumbers`, partsLinkNumbers);
    }

    public createVendorCatalog(vendorCatalog: VendorCatalog) : Observable<VendorCatalog[]> {
        return this.http.post<VendorCatalog[]>(`${environment.apiUrl}/${this.url}/CreateVendorCatalog`, vendorCatalog);
    }

    public createVendorCatalogByProduct(productVendorCatalog: ProductVendorCatalog) : Observable<VendorCatalog[]> {
        return this.http.post<VendorCatalog[]>(`${environment.apiUrl}/${this.url}/CreateVendorCatalogByProduct`, productVendorCatalog);
    }

    public updateVendorCatalog(vendorCatalog: VendorCatalog) : Observable<VendorCatalog[]> {
        return this.http.put<VendorCatalog[]>(`${environment.apiUrl}/${this.url}/UpdateVendorCatalog`, vendorCatalog);
    }

    public updateVendorCatalogByProduct(productVendorCatalog: ProductVendorCatalog) : Observable<VendorCatalog[]> {
        return this.http.put<VendorCatalog[]>(`${environment.apiUrl}/${this.url}/updateVendorCatalogByProduct`, productVendorCatalog);
    }

    public deleteVendorCatalog(vendorCatalogs: VendorCatalog[]) : Observable<VendorCatalog[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: vendorCatalogs.map(a => a.id),
          };
          
        return this.http.delete<VendorCatalog[]>(`${environment.apiUrl}/${this.url}/DeleteVendorCatalog`, options);
    }
}