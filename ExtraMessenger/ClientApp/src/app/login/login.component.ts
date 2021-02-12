import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-counter-component',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;

  constructor(
    private _formBuilder: FormBuilder
  ) {
  }

  private createLoginForm(): void {
    this.loginForm = this._formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  login(): void {

  }

  ngOnInit(): void {
    this.createLoginForm();
  }
}
