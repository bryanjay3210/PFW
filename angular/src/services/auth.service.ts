import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { LoginResponse } from "src/services/interfaces/auth/loginresponse.model";
import { environment } from "src/environments/environment";
import { LoginDTO } from "./interfaces/auth/logindto.model";
import { RegistrationResponse } from "./interfaces/auth/registrationresponse.model";
import { UserDTO } from "./interfaces/models";

@Injectable({
    providedIn: 'root'
})

export class AuthService {
    url = "Auth"
    constructor(private http: HttpClient) {}

    public register(userdto = {} as UserDTO) : Observable<RegistrationResponse> {
        return this.http.post<RegistrationResponse>(`${environment.apiUrl}/${this.url}/Register`, userdto);
    }

    public login(logindto: LoginDTO) : Observable<LoginResponse> {
        return this.http.post<LoginResponse>(`${environment.apiUrl}/${this.url}/Login`, logindto);
    }

    // public login(userdto: UserDTO) : Observable<string> {
    //     const options = {
    //         headers: new HttpHeaders({
    //         'Content-Type': 'application/json',
    //         }),
            
    //         responseType: 'text',
    //         body: { username: userdto.userName, password: userdto.password },
    //     };
          
    //     // const headers = new HttpHeaders({
    //     //     'Accept': 'application/json',
    //     //     'Accept-Language': 'en_US',
    //     //     'Content-Type': 'application/x-www-form-urlencoded',
    //     //     'Authorization': 'Basic Y2xpZW50X2lkOnNlY3JldA=='
    //     //   });
    //     //   this.http.post('https://api.sandbox.paypal.com/v1/oauth2/token', 'grant_type=client_credentials', { headers: headers });

    //     return this.http.post<string>(`${environment.apiUrl}/${this.url}/Login`, options);  
    // }
}