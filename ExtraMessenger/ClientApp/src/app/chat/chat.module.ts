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

@NgModule({
  declarations: [
    MessageSegmentComponent,
    SidebarComponent,
    ChatComponent,
    UsersComponent,
    RouterComponent,
    DialogOverviewExampleDialog
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
    FormsModule
  ]
})
export class ChatModule { }
