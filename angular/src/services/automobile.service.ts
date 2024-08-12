import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { Automobile } from "./interfaces/automobile.model";

@Injectable({
    providedIn: 'root'
})

export class AutomobileService {
    url = "Automobile"
    constructor(private http: HttpClient) {}

    public getAutomobiles() : Observable<Automobile[]> {
        return this.http.get<Automobile[]>(`${environment.apiUrl}/${this.url}/GetAutomobiles`);
    }

    public createAutomobile(automobile: Automobile) : Observable<Automobile[]> {
        return this.http.post<Automobile[]>(`${environment.apiUrl}/${this.url}/CreateAutomobile`, automobile);
    }

    public updateAutomobile(automobile: Automobile) : Observable<Automobile[]> {
        return this.http.put<Automobile[]>(`${environment.apiUrl}/${this.url}/UpdateAutomobile`, automobile);
    }

    public deleteAutomobile(automobiles: Automobile[]) : Observable<Automobile[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: automobiles.map(a => a.id),
          };
          
        return this.http.delete<Automobile[]>(`${environment.apiUrl}/${this.url}/DeleteAutomobile`, options);
    }
}