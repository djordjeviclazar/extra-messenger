import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MessageService } from 'src/app/_services/message.service';
import { AuthService } from '../../_services/auth.service';

@Component({
  selector: 'app-message-segment',
  templateUrl: './message-segment.component.html',
  styleUrls: ['./message-segment.component.css']
})
export class MessageSegmentComponent implements OnInit {

  messages$: Observable<any[]>; //merge operator
  messages: any[] = [];
  messagesSubject = new BehaviorSubject<any[]>([]);
  threadChange$;
  socketSubscription: Subscription;
  messageToSend: string;

  constructor(
    public _authService: AuthService,
    public _messageService: MessageService
  ) { } //, private _authService: AuthService

  ngOnInit(): void {
    this.messages$ = this.messagesSubject.asObservable();
    this.socketSubscription = this._messageService.messageArrived.asObservable() 
      .subscribe(
        data => {
          if (data) {
            if (this._messageService.messageThread.value.chatInteractionId === data.chatInteractionId) {
              this.messages.unshift(data.message);
              this.messagesSubject.next(this.messages)
            } else {
              // notification
            }
          }
        }
      )

    this.threadChange$ = this._messageService.messageThread
      .asObservable().subscribe(data => {
        this._messageService.getMessages()?.subscribe(data => {
          this.messagesSubject.next(data)
          this.messages = data
        });

      });
  }

  ngOnDestroy(): void {
    this.threadChange$.unsubscribe();
    // this.socketSubscription.unsubscribe();
  }

  sendMessage() {
    if (this._messageService.messageThread.value == undefined ||
      this._messageService.messageThread.value.recieverId == undefined ||
      this._messageService.messageThread.value.chatInteractionId == undefined)
      return;

    if (this.messageToSend === '')
      return;

    this._messageService.sendMessage(this._messageService.messageThread.value.recieverId,
      this.messageToSend,
      this._messageService.messageThread.value.chatInteractionId);
    this.messageToSend = '';
  }
}
