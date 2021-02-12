import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MessageService } from 'src/app/services/message.service';

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

  constructor(public _messageService: MessageService) { } //, private _authService: AuthService

  ngOnInit(): void {

    this.messages$ = this.messagesSubject.asObservable();

    this.socketSubscription = this._messageService.messageArrived.asObservable()
      .pipe(
        filter(data => data.senderId == this._messageService.messageThread.value.recieverId ) //|| this._authService._decodedToken.nameid == data.senderId
      ).subscribe(
        data => {
          this.messages.unshift(data);
          this.messagesSubject.next(this.messages)
        }
      )

    this.threadChange$ = this._messageService.messageThread
      .asObservable().subscribe(data => {
        this._messageService.getMessages().subscribe(data => {
          this.messagesSubject.next(data)
          this.messages = data
        });

      });
  }

  ngOnDestroy(): void {
    this.threadChange$.unsubscribe();
    // this.socketSubscription.unsubscribe();
  }
}
