import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MessageService } from 'src/app/_services/message.service';
import { AuthService } from '../../_services/auth.service';
import { DeletedMessageDto } from '../../_DTOs/deletedMessageDto';
import { EditedMessageDto } from '../../_DTOs/editedMessageDto';
import { ReceivedMessageDto } from '../../_DTOs/receivedMessageDto';
import { MessageReturnDto } from '../../_DTOs/messageReturnDto';
import { MatDialog } from '@angular/material/dialog';
import { EditMessageDialogComponent } from './edit-message-dialog/edit-message-dialog.component';
import { DeleteMessageDialogComponent } from './delete-message-dialog/delete-message-dialog.component';

@Component({
  selector: 'app-message-segment',
  templateUrl: './message-segment.component.html',
  styleUrls: ['./message-segment.component.css']
})
export class MessageSegmentComponent implements OnInit {

  messages$: Observable<any[]>; //merge operator
  messages: MessageReturnDto[] = [];
  messagesSubject = new BehaviorSubject<any[]>([]);
  threadChange$;
  socketSubscription: Subscription;
  messageToSend: string;
  showEditDeleteMessage;

  constructor(
    private _matDialog: MatDialog,
    public _authService: AuthService,
    public _messageService: MessageService
  ) { } 

  ngOnInit(): void {
    this.messages$ = this.messagesSubject.asObservable();
    this.socketSubscription = this._messageService.messageArrived.asObservable()
      .subscribe(
        (data: ReceivedMessageDto) => {
          if (data) {
            if (data.edited)
              this.processEditedMessage(data);
            else if (data.deleted)
              this.processDeletedMessage(data);
            else
              this.processReceivedMessage(data);
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
  processReceivedMessage(data: ReceivedMessageDto) {
    if (this._messageService.messageThread.value.chatInteractionId === data.chatInteractionId) {
      this.messages.unshift(data.message);
      this.messagesSubject.next(this.messages)
    } else {
      // notification
    }
  }

  processDeletedMessage(data: ReceivedMessageDto) {
    if (this._messageService.messageThread.value.chatInteractionId === data.chatInteractionId) {
      this.messages = this.messages.filter(message => message.id !== data.message.id);
      this.messagesSubject.next(this.messages)
    } else {
      // notification
    }
  }

  processEditedMessage(data: ReceivedMessageDto) {
    if (this._messageService.messageThread.value.chatInteractionId === data.chatInteractionId) {
      this.messages = this.messages.map(message => {
        if (message.id === data.message.id)
          message.content = data.message.content;
        return message;
      });
      this.messagesSubject.next(this.messages)
    } else {
      // notification
    }
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

  editMessage(message: MessageReturnDto) {
    const dialogRef = this._matDialog.open(EditMessageDialogComponent, {
      maxHeight: '250px',
      width: '300px',
      data: message
    });

    dialogRef.afterClosed().subscribe((editedMessage: MessageReturnDto) => {
      if (message.content !== editedMessage.content)
        this._messageService.editMessage(editedMessage,
          this._messageService.messageThread.value.recieverId,
          this._messageService.messageThread.value.chatInteractionId);
    });
  }

  deleteMessage(message: MessageReturnDto) {
    const dialogRef = this._matDialog.open(DeleteMessageDialogComponent, {
      maxHeight: '550px',
      width: '450px',
      data: message
    });

    dialogRef.afterClosed().subscribe((shouldDelete: boolean) => {
      if (shouldDelete)
        this._messageService.deleteMessage(message.id, this._messageService.messageThread.value.recieverId,
          this._messageService.messageThread.value.chatInteractionId);
    });
  }
}
