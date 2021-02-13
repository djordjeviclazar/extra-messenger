import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { RegisterComponent } from '../register/register.component';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  registerDialogRef: MatDialogRef<RegisterComponent>;

  constructor(
    private _matDialog: MatDialog,
    private _router: Router,
    public _authService: AuthService
  ) {

  }

  ngOnInit(): void {
  }

  register() {
    if (this.registerDialogRef) return;
    this.registerDialogRef = this._matDialog.open(RegisterComponent, {
      width: '550px',
      height: '400px',
      autoFocus: false
    });

    this.registerDialogRef.afterClosed().subscribe(() => {
      this.registerDialogRef = undefined;
    });
  }

  openChat() {
    this._router.navigate(['/chat']);
  }
}
