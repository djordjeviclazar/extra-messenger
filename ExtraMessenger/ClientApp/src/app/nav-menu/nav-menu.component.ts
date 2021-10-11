import {Component, OnInit} from '@angular/core';
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {LoginComponent} from "../login/login.component";
import {finalize} from "rxjs/operators";
import { RegisterComponent } from '../register/register.component';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  loginDialogRef: MatDialogRef<LoginComponent>;
  registerDialogRef: MatDialogRef<RegisterComponent>;

  constructor(
    private _matDialog: MatDialog,
    private _router: Router,
    private _alertifyService: AlertifyService,
    public _authService: AuthService
  ) {
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  openLoginDialog(): void {
    if (this.loginDialogRef) return;
    if (this.registerDialogRef)
      this.registerDialogRef.close();
    this.loginDialogRef = this._matDialog.open(LoginComponent, {
      width: '450px',
      height: '270px',
    });

    this.loginDialogRef.afterClosed().subscribe(() => {
      this.loginDialogRef = undefined;
    });
  }

  openRegisterDialog(): void {
    if (this.registerDialogRef) return;
    if (this.loginDialogRef)
      this.loginDialogRef.close();
    this.registerDialogRef = this._matDialog.open(RegisterComponent, {
      width: '550px',
      height: '400px',
      autoFocus: false
    });

    this.registerDialogRef.afterClosed().subscribe(() => {
      this.registerDialogRef = undefined;
    });
  }


  ngOnInit(): void {
  }

  logout() {
    this._router.navigate(['']);
    this._authService.logout();
    this._alertifyService.message("Successfully logged out");
  }

  navigateChat() {
    this._router.navigate(['/chat']);
  }

  navigateUsers() {
    this._router.navigate(['/chat/users']);
  }
  
  navigateRepoFetch() {
    this._router.navigate(['repo/fetchrepo']);
  }
}
