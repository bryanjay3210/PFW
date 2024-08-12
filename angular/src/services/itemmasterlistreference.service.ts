import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { ItemMasterlistReference } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class ItemMasterlistReferenceService {
    url = "ItemMasterlistReference"
    constructor(private http: HttpClient) {}

    public getItemMasterlistReferences() : Observable<ItemMasterlistReference[]> {
        return this.http.get<ItemMasterlistReference[]>(`${environment.apiUrl}/${this.url}/GetItemMasterlistReferences`);
    }

    public getItemMasterlistReferencesByProductId(productId: number) : Observable<ItemMasterlistReference[]> {
        return this.http.get<ItemMasterlistReference[]>(`${environment.apiUrl}/${this.url}/GetItemMasterlistReferencesByProductId?productId=${productId}`);
    }

    public createItemMasterlistReference(itemMasterlistReference: ItemMasterlistReference) : Observable<ItemMasterlistReference[]> {
        return this.http.post<ItemMasterlistReference[]>(`${environment.apiUrl}/${this.url}/CreateItemMasterlistReference`, itemMasterlistReference);
    }

    public createItemMasterlistReferenceByProduct(itemMasterlistReference: ItemMasterlistReference) : Observable<ItemMasterlistReference[]> {
        return this.http.post<ItemMasterlistReference[]>(`${environment.apiUrl}/${this.url}/CreateItemMasterlistReferenceByProduct`, itemMasterlistReference);
    }

    public updateItemMasterlistReference(itemMasterlistReference: ItemMasterlistReference) : Observable<ItemMasterlistReference[]> {
        return this.http.put<ItemMasterlistReference[]>(`${environment.apiUrl}/${this.url}/UpdateItemMasterlistReference`, itemMasterlistReference);
    }

    public updateItemMasterlistReferenceByProduct(itemMasterlistReference: ItemMasterlistReference) : Observable<ItemMasterlistReference[]> {
        return this.http.put<ItemMasterlistReference[]>(`${environment.apiUrl}/${this.url}/UpdateItemMasterlistReferenceByProduct`, itemMasterlistReference);
    }

    public deleteItemMasterlistReference(itemMasterlistReferences: ItemMasterlistReference[]) : Observable<ItemMasterlistReference[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: itemMasterlistReferences.map(a => a.id),
          };
          
        return this.http.delete<ItemMasterlistReference[]>(`${environment.apiUrl}/${this.url}/DeleteItemMasterlistReference`, options);
    }
}