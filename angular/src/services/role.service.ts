import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { Role } from "./interfaces/role.model";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class RoleService {
    url = "Role"
    constructor(private http: HttpClient) {}

    public getRoles() : Observable<Role[]> {
        return this.http.get<Role[]>(`${environment.apiUrl}/${this.url}/GetRoles`);
    }

    public getRoleById(id: number) : Observable<Role> {
        return this.http.get<Role>(`${environment.apiUrl}/${this.url}/GetRoleById`);
    }

    public createRole(role: Role) : Observable<Role[]> {
        return this.http.post<Role[]>(`${environment.apiUrl}/${this.url}/CreateRole`, role);
    }

    public updateRole(role: Role) : Observable<Role[]> {
        return this.http.put<Role[]>(`${environment.apiUrl}/${this.url}/UpdateRole`, role);
    }

    public deleteRole(roles: Role[]) : Observable<Role[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: roles.map(a => a.id),
          };
          
        return this.http.delete<Role[]>(`${environment.apiUrl}/${this.url}/DeleteRole`, options);
    }
}