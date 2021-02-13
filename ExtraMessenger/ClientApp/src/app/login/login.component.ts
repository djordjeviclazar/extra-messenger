import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import { MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-counter-component',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;

  constructor(
    private _formBuilder: FormBuilder,
    private _authService: AuthService,
    private _alertifyService: AlertifyService,
    private _router: Router,
    public _dialogReft: MatDialogRef<LoginComponent>
  ) {
  }

  private createLoginForm(): void {
    this.loginForm = this._formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  login(): void {
    const userLoginDto = Object.assign({}, this.loginForm.value);;
    if (!this.loginForm.valid)
      return;

    this._authService.login(userLoginDto).subscribe((response: any) => {
      const loginResponse: any = Object.assign({}, response);;
      if (!loginResponse.status) {
        this._alertifyService.error(loginResponse.message ?? "Invalid username/password combination.");
      } else {
        this._alertifyService.success(loginResponse.message ?? "Successfully logged in.");
        this._dialogReft.close();
        this._router.navigate(["/chat"]);
      }
    },
      error => {
        this._alertifyService.error("Unknown error.");
        console.log(error.message);
      }
    );
  }

  ngOnInit(): void {
    this.createLoginForm();
  }
}
