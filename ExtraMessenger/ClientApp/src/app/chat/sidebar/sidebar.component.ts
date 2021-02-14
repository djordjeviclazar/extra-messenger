import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {

  contacts$: Observable<any[]>;

  constructor(private _messageService: MessageService) { }

  ngOnInit(): void {
    this.contacts$ = this._messageService.getContacts();/*.pipe(
          tap(res => console.log(res))
      );*/
  }

  openThread(chatInteractionId, receiverName) {
    this._messageService.messageThread.next({chatInteractionId: chatInteractionId, recieverId: receiverName});
  }

}
