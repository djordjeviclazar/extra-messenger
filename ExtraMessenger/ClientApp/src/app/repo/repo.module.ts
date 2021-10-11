import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { HttpClientModule } from '@angular/common/http';
import { FetchreposComponent } from './fetchrepos/fetchrepos.component';
import { ReporouterComponent } from './reporouter.component';



@NgModule({
  declarations: [FetchreposComponent],
  imports: [
    CommonModule,
    RouterModule.forChild([
      {
        path: '', component: FetchreposComponent,
        children: [
          //{ path: '', component: FetchreposComponent, pathMatch: 'prefix' },
          { path: 'fetchrepo', component: FetchreposComponent, pathMatch: 'prefix' }
        ]
      }
    ]),
    MatCardModule,
    MatButtonModule,
    MatDialogModule,
    FormsModule,
    MatIconModule,
    ReactiveFormsModule,
    MatInputModule,
    MatFormFieldModule,
    HttpClientModule
  ]
})
export class RepoModule { }
