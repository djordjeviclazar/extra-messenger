import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  registerForm: FormGroup;

  constructor(
    private _formBuilder: FormBuilder,
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

