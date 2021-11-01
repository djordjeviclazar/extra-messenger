import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from "@angular/material/dialog";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { LoginComponent } from "./login/login.component";
import { RegisterComponent } from './register/register.component';
import { AuthGuard } from './_services/_guards/auth.guard';
import { MatDividerModule } from '@angular/material/divider';
import { JwtModule } from '@auth0/angular-jwt';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    LoginComponent,
    RegisterComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    JwtModule.forRoot({
      config: {
         tokenGetter: function tokenGetter() {
            return localStorage.getItem('authToken');
         },
         allowedDomains:['localhost:5000', 'localhost:5001'],
         disallowedRoutes: ['localhost:5000/authentication', 'localhost:5001/authentication']
      }
   }),
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      {
        path: 'chat',
        loadChildren: () => import('./chat/chat.module').then(m => m.ChatModule),
        canActivate: [AuthGuard]
      },
      {
        path: 'repo',
        loadChildren: () => import('./repo/repo.module').then(m => m.RepoModule),
        canActivate: [AuthGuard]
      },
    ]),
    BrowserAnimationsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatButtonModule,
    ReactiveFormsModule,
    MatTooltipModule,
    MatDialogModule,
    MatCheckboxModule,
    MatDividerModule
  ],
    providers: [],
    bootstrap: [AppComponent]
  })
export class AppModule {
}
