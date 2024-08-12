import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { environment } from "src/environments/environment";
import { SessionTimeoutDTO, User, UserNotificationDTO } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class UserService {
    url = "User"
    constructor(private http: HttpClient) {}

    public getUsers() : Observable<User[]> {
        return this.http.get<User[]>(`${environment.apiUrl}/${this.url}/GetUsers`);
    }

    public getUserNotificationsByUserId(userId: Number) : Observable<UserNotificationDTO> {
        return this.http.get<UserNotificationDTO>(`${environment.apiUrl}/${this.url}/GetUserNotificationsByUserId?userId=${userId}`);
    }
    
    public getSessionTimeout() : Observable<SessionTimeoutDTO> {
        return this.http.get<SessionTimeoutDTO>(`${environment.apiUrl}/${this.url}/GetSessionTimeout`);
    }

    // public getUserByEmail(userdto: UserDTO) : Observable<User[]> {
    //     return this.http.get<User[]>(`${environment.apiUrl}/${this.url}/GetUserByEmail`, userdto);
    // }

    public createUser(user = {} as User) : Observable<User[]> {
        return this.http.post<User[]>(`${environment.apiUrl}/${this.url}/CreateUser`, user);
    }

    public updateUser(user = {} as User) : Observable<User[]> {
        return this.http.put<User[]>(`${environment.apiUrl}/${this.url}/UpdateUser`, user);
    }

    public deleteUser(users = {} as User[]) : Observable<User[]> {
        const options = {
            headers: new HttpHeaders({
              'Content-Type': 'application/json',
            }),
            body: users.map(a => a.id),
          };
          
        return this.http.delete<User[]>(`${environment.apiUrl}/${this.url}/DeleteUser`, options);
    }
}
