import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MessageSegmentComponent } from './message-segment/message-segment.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { ChatComponent } from './chat/chat.component';
import { RouterModule } from '@angular/router';
import { MatDividerModule } from '@angular/material/divider';
import { DialogOverviewExampleDialog, UsersComponent } from './users/users.component';
import { RouterComponent } from './router.component';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { EditMessageDialogComponent } from './message-segment/edit-message-dialog/edit-message-dialog.component';
import { DeleteMessageDialogComponent } from './message-segment/delete-message-dialog/delete-message-dialog.component';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

@NgModule({
  declarations: [
    MessageSegmentComponent,
    SidebarComponent,
    ChatComponent,
    UsersComponent,
    RouterComponent,
    DialogOverviewExampleDialog,
    EditMessageDialogComponent,
    DeleteMessageDialogComponent
  ],
  imports: [
    CommonModule,
    MatDividerModule,
    RouterModule.forChild([
      {
        path: '', component: RouterComponent,
        children: [
          { path: '', component: ChatComponent, pathMatch: 'full' },
          { path: 'users', component: UsersComponent, pathMatch: 'full' }
        ]
      }
    ]),
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatDialogModule,
    FormsModule,
    MatIconModule,
    ReactiveFormsModule,
    MatInputModule,
    MatFormFieldModule,
  ]
})
export class ChatModule { }
