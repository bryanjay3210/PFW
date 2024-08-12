import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { Category, Sequence } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class SequenceService {
    url = "Sequence"
    constructor(private http: HttpClient) {}

    public getSequences() : Observable<Sequence[]> {
        return this.http.get<Sequence[]>(`${environment.apiUrl}/${this.url}/GetSequences`);
    }
    public getSequencesByCategoryId(categoryId: number) : Observable<Category[]> {
        return this.http.get<Category[]>(`${environment.apiUrl}/${this.url}/GetSequencesByCategoryId?categoryIds=${categoryId}`);
    }
    public createSequence(sequence: Sequence) : Observable<Sequence[]> {
        return this.http.post<Sequence[]>(`${environment.apiUrl}/${this.url}/CreateSequence`, sequence);
    }

    public updateSequence(sequence: Sequence) : Observable<Sequence[]> {
        return this.http.put<Sequence[]>(`${environment.apiUrl}/${this.url}/UpdateSequence`, sequence);
    }

    public deleteSequence(sequences: Sequence[]) : Observable<Sequence[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: sequences.map(a => a.id),
          };
          
        return this.http.delete<Sequence[]>(`${environment.apiUrl}/${this.url}/DeleteSequence`, options);
    }
}