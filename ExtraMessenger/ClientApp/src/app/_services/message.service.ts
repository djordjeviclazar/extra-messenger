import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject, Observable } from 'rxjs';
import { isNullOrUndefined } from 'util';
import { environment } from '../../environments/environment';
import { MessageReturnDto } from '../_DTOs/messageReturnDto';
import { ReceivedMessageDto } from '../_DTOs/receivedMessageDto';
import { AlertifyService } from './alertify.service';
import { AuthService } from './auth.service';
@Injectable({
  providedIn: 'root'
})
export class MessageService {
  _baseUrl = 'http://localhost:5000/';
  _message: string = '';
  _hubConnection: signalR.HubConnection;
  _receivers: any[] = [];
  currentChatInteraction: string;
  messageThread = new BehaviorSubject<{ recieverId: string, chatInteractionId: string }>({
    recieverId: "-2",
    chatInteractionId: "-2"
  });
  messageArrived = new BehaviorSubject<any>(null);

  public startConnection = () => {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this._baseUrl + 'chat', { accessTokenFactory: () => localStorage.getItem('authToken') })
      .build();

    this._hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ', err));
  }

  public sendMessage = (receiverId: string, message: any, chatInteractionId: string = undefined) => {
    if (receiverId == null) { return; }
    let msgObject = { message: message, chatInteractionId: chatInteractionId };

    if (this._hubConnection.state == 0) {
    //   this._translator.get('somethingWentWrong').subscribe(res => this._alertifyService.error(res));
    }

    this._hubConnection.send("sendMessage", receiverId, msgObject)
      .then()
      .catch();
  }

  public deleteMessage = (messageId: string, receiverId: string, chatInteractionId: string) => {
    let msgObject = { id: messageId, chatInteractionId: chatInteractionId };

    this._hubConnection.send("deleteMessage", receiverId, msgObject)
      .then()
      .catch();
  }

  public editMessage = (message: MessageReturnDto, receiverId: string, chatInteractionId: string) => {
    const editMessageDto = {
      id: message.id,
      message: message.content,
      chatInteractionId: chatInteractionId
    };

    this._hubConnection.send("editMessage", receiverId, editMessageDto)
      .then()
      .catch();
  }

  public addRecievedMessageListener = () => {
    this._hubConnection.on('receivedMessage', (receivedMessageDto: ReceivedMessageDto) => {
      this.messageArrived.next(receivedMessageDto);
      if(this.currentChatInteraction != receivedMessageDto.chatInteractionId)
        this.alertify.message('New Message From: ' + receivedMessageDto.message.sender)
    })

    this._hubConnection.on('editedMessage', (editedMessageDto: ReceivedMessageDto) => {
      editedMessageDto.edited = true;
      this.messageArrived.next(editedMessageDto);
    })

    this._hubConnection.on('deletedMessage', (deletedMessageDto: ReceivedMessageDto) => {
      deletedMessageDto.deleted = true;
      this.messageArrived.next(deletedMessageDto);
    })
  }

  public disconnect() {
    this._hubConnection.stop();
  }

  constructor(
    private _http: HttpClient,
    private _authService: AuthService,
    private alertify: AlertifyService
  ) {
  }

  setReceiveMessageHandler(receiveMessageHandler: any, userId: string) {
    let newReceiver = { id: userId, handler: receiveMessageHandler };
    this._receivers.push(newReceiver);
  }

  getContacts(): import("rxjs").Observable <any[]> { //import("../_models/chatInteraction").
    if (isNullOrUndefined(this._authService._decodedToken))
      this._authService._decodedToken = new JwtHelperService().decodeToken(localStorage.getItem('authToken'));
    return this._http.get<any[]>(environment.apiUrl + 'messenger/contacts/' + this._authService._decodedToken.unique_name); // get by username
  }

  getMessages(): Observable<any[]> {
    if (this.messageThread.value == undefined ||
      this.messageThread.value.recieverId == undefined ||
      this.messageThread.value.chatInteractionId == undefined)
      return;

    if (Number.parseInt(this.messageThread.value.recieverId) < 0)
      return;

    return this._http.get<any[]>(environment.apiUrl + 'messenger/messages/' + this.messageThread.value.chatInteractionId);
  }

}
