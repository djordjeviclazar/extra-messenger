import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { map } from 'rxjs/operators';
import { AlertifyService } from './alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  _baseUrl = environment.apiUrl;
  _jwtHelper = new JwtHelperService();
  _decodedToken: any;

  constructor(private _http: HttpClient, private _alertify: AlertifyService) { }

  login(user) {
    return this._http.post(this._baseUrl + 'authentication/login', user)
      .pipe(
        map((response: any) => {
          if (response.status) {
            localStorage.setItem('authToken', response.token);
            this._decodedToken = this._jwtHelper.decodeToken(response.token);
          }
          return response;
        })
      );
  }

  register(user: any) {
    return this._http.post(this._baseUrl + 'authentication/register', user);
  }

  loggedIn() {
    const tokenFromStorage = localStorage.getItem('authToken');
    return !this._jwtHelper.isTokenExpired(tokenFromStorage);
  }

  logout() {
    localStorage.removeItem('authToken');
  }

}
