import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { tap } from 'rxjs/operators';
import { isNullOrUndefined } from 'util';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  registerForm: FormGroup;
  _jwtHelper = new JwtHelperService();

  constructor(
    private _formBuilder: FormBuilder,
    private _authService: AuthService,
    private _alertifyService: AlertifyService,
    private _router: Router,
    
    public dialogRef: MatDialogRef<RegisterComponent>
  ) {
  }

  ngOnInit(): void {
    this.createRegisterForm();
  }

  private createRegisterForm(): void {
    const fcPass: FormControl = new FormControl('');
    const fcConfirmPass: FormControl = new FormControl('', [Validators.required, this.passwordMatchValidator(fcPass)]);
    fcPass.setValidators([Validators.required, this.updateConfirmPasswordValidity(fcConfirmPass)]);
    this.registerForm = this._formBuilder.group({
      username: ['', Validators.required],
      password: fcPass,
      confirmPassword: fcConfirmPass
    });
  }

  register() {
    if (!this.registerForm.valid)
      return;
    const userRegisterFormValue = Object.assign({}, this.registerForm.value);
    const userRegisterDto = Object.assign({ username: userRegisterFormValue.username, password: userRegisterFormValue.password });
    this._authService.register(userRegisterDto).pipe(
      tap((response: any) => {
        const loginResponse = Object.assign({}, response);
        if (!loginResponse.status) {
          this._alertifyService.error(loginResponse.message ?? "Registration failed.");
        } else {
          this._alertifyService.success(loginResponse.message ?? "Registration successful. Logging in...");
          this.dialogRef.close();
          localStorage.setItem('authToken', response.token);
          this._authService._decodedToken = this._jwtHelper.decodeToken(response.token);
          this._router.navigate(["/chat"]);
          this._alertifyService.success(loginResponse.message ?? "Successfully logged in");
        }
      },
        error => {
          if (error.error && error.error?.message !== '')
            this._alertifyService.error(error.error.message);
          else
            this._alertifyService.error("Unknown error.");
          console.log(error.message);
        }
      )).subscribe();
  }

  cancel() {
    this.dialogRef.close();
  }

  passwordMatchValidator(passControl: AbstractControl): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const passwordValue: string = passControl.value;
      const toRet = (control.value !== passwordValue)
        ? { 'passwordMatch': true }
        : null;

      return toRet;
    };
  }

  updateConfirmPasswordValidity(confirmPassControl: AbstractControl): ValidatorFn {
    return (control: AbstractControl) => {
      confirmPassControl.updateValueAndValidity();
      return null;
    };
  }

}

