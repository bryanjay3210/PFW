import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { PartsCatalog } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class PartsCatalogService {
    url = "PartsCatalog"
    constructor(private http: HttpClient) {}

    public getPartsCatalogs() : Observable<PartsCatalog[]> {
        return this.http.get<PartsCatalog[]>(`${environment.apiUrl}/${this.url}/GetPartsCatalogs`);
    }

    public getPartsCatalogsByProductId(productId: number) : Observable<PartsCatalog[]> {
        return this.http.get<PartsCatalog[]>(`${environment.apiUrl}/${this.url}/GetPartsCatalogsByProductId?productId=${productId}`);
    }

    public createPartsCatalog(product: PartsCatalog) : Observable<PartsCatalog[]> {
        return this.http.post<PartsCatalog[]>(`${environment.apiUrl}/${this.url}/CreatePartsCatalog`, product);
    }

    public updatePartsCatalog(product: PartsCatalog) : Observable<PartsCatalog[]> {
        return this.http.put<PartsCatalog[]>(`${environment.apiUrl}/${this.url}/UpdatePartsCatalog`, product);
    }

    public deletePartsCatalog(products: PartsCatalog[]) : Observable<PartsCatalog[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: products.map(a => a.id),
          };
          
        return this.http.delete<PartsCatalog[]>(`${environment.apiUrl}/${this.url}/DeletePartsCatalog`, options);
    }
}