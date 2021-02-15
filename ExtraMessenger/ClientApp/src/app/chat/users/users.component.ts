import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { map, tap } from 'rxjs/operators';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {

  users$ = this.router.get<any[]>('https://localhost:5001/api/user/explore', {
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
    }
  }).pipe(
    map(users => users.map(u => {
        let user = Math.floor(Math.random() * 100) + 1 > 50 ? 'men' : 'women';
        u.image = `https://randomuser.me/api/portraits/${user}/${Math.floor(Math.random() * 50) + 1}.jpg`
        return u;
      }))
  )

  constructor(private router: HttpClient, public dialog: MatDialog) { }

  ngOnInit(): void {
  }

  openChatModal(objectId) {
    const dialogRef = this.dialog.open(DialogOverviewExampleDialog, {
      width: '250px',
      data: {objectId: objectId}
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

}

@Component({
  selector: 'dialog-overview-example-dialog',
  templateUrl: 'dialog-overview-example-dialog.html',
})
export class DialogOverviewExampleDialog {
  message: string;

  constructor(
    public dialogRef: MatDialogRef<DialogOverviewExampleDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any, private messageService: MessageService) {}

  onNoClick(): void {
    this.dialogRef.close();
  }

  sendMessage() {
    // this.http.post('', {message: this.message, chatInteractionId: null})
    this.messageService.sendMessage(this.data.objectId, this.message)
    this.dialogRef.close();
  }
}
