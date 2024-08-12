import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { Module } from "./interfaces/module.model";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class ModuleService {
    url = "Module"
    constructor(private http: HttpClient) {}

    public getModules() : Observable<Module[]> {
        return this.http.get<Module[]>(`${environment.apiUrl}/${this.url}/GetModules`);
    }

    public createModule(module: Module) : Observable<Module[]> {
        return this.http.post<Module[]>(`${environment.apiUrl}/${this.url}/CreateModule`, module);
    }

    public updateModule(module: Module) : Observable<Module[]> {
        return this.http.put<Module[]>(`${environment.apiUrl}/${this.url}/UpdateModule`, module);
    }

    public deleteModule(modules: Module[]) : Observable<Module[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: modules.map(a => a.id),
          };
          
        return this.http.delete<Module[]>(`${environment.apiUrl}/${this.url}/DeleteModule`, options);
    }
}