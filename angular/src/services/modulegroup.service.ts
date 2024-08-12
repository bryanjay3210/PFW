import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { ModuleGroup } from "./interfaces/moduleGroup.model";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class ModuleGroupService {
    url = "ModuleGroup"
    constructor(private http: HttpClient) {}

    public getModuleGroups() : Observable<ModuleGroup[]> {
        return this.http.get<ModuleGroup[]>(`${environment.apiUrl}/${this.url}/GetModuleGroups`);
    }

    public createModuleGroup(moduleGroup: ModuleGroup) : Observable<ModuleGroup[]> {
        return this.http.post<ModuleGroup[]>(`${environment.apiUrl}/${this.url}/CreateModuleGroup`, moduleGroup);
    }

    public updateModuleGroup(moduleGroup: ModuleGroup) : Observable<ModuleGroup[]> {
        return this.http.put<ModuleGroup[]>(`${environment.apiUrl}/${this.url}/UpdateModuleGroup`, moduleGroup);
    }

    public deleteModuleGroup(moduleGroups: ModuleGroup[]) : Observable<ModuleGroup[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: moduleGroups.map(a => a.id),
          };
          
        return this.http.delete<ModuleGroup[]>(`${environment.apiUrl}/${this.url}/DeleteModuleGroup`, options);
    }
}