import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
//import { Category } from "./interfaces/category.model";
import { environment } from "src/environments/environment";
import { Category } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class CategorytService {
    url = "Category"
    constructor(private http: HttpClient) {}

    public getCategories() : Observable<Category[]> {
        return this.http.get<Category[]>(`${environment.apiUrl}/${this.url}/GetCategories`);
    }

    public createCategory(category: Category) : Observable<Category[]> {
        return this.http.post<Category[]>(`${environment.apiUrl}/${this.url}/CreateCategory`, category);
    }

    public updateCategory(category: Category) : Observable<Category[]> {
        return this.http.put<Category[]>(`${environment.apiUrl}/${this.url}/UpdateCategory`, category);
    }

    public deleteCategory(categorys: Category[]) : Observable<Category[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: categorys.map(a => a.id),
          };
          
        return this.http.delete<Category[]>(`${environment.apiUrl}/${this.url}/DeleteCategory`, options);
    }
}