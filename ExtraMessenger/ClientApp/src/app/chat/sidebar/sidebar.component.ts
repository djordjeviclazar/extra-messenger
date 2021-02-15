import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {

  contacts$: Observable<any[]>;
  clickedContact = null;

  constructor(private _messageService: MessageService) { }
  ngOnInit(): void {
    this.contacts$ = this._messageService.getContacts().pipe(
      tap(contacts => {
        if (contacts?.length > 0)
          this.openThread(contacts[0].chatInteractionReference, contacts[0].otherUserId);
      }
      )
    );
  }

  openThread(chatInteractionId, receiverId) {
    this.clickedContact = chatInteractionId;
    this._messageService.messageThread.next({ chatInteractionId: chatInteractionId, recieverId: receiverId });
  }

}
