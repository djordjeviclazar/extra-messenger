import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { AuthService } from '../auth.service';
import { AlertifyService } from '../alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private _authService: AuthService,
    private _router: Router,
    private _alertifyService: AlertifyService,
  ) { }

  canActivate(route: ActivatedRouteSnapshot): boolean {
    if (this._authService.loggedIn() && !route.data.forLoggedIn) {
      this._router.navigate(["/promotions"]);
      return false;
    }

    if (!this._authService.loggedIn() && route.data.forLoggedIn) {
      this._router.navigate(["/"]);
      return false;
    }

    return true;
  }
}
