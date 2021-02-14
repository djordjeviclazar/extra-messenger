import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject, Observable } from 'rxjs';
import { isNullOrUndefined } from 'util';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
@Injectable({
  providedIn: 'root'
})
export class MessageService {
  _baseUrl = 'http://localhost:5000/';
  _message: string = '';
  _hubConnection: signalR.HubConnection;
  _receivers: any[] = [];
  messageThread = new BehaviorSubject<{ recieverId: string, chatInteractionId: string }>({
    recieverId: "-2",
    chatInteractionId: "-2"
  });
  messageArrived = new BehaviorSubject<any>({
    text: '',
    timeSent: new Date(),
    senderUsername: '',
    receiverUsername: '',
    seen: true,
    senderId: "-1",
    recieverId: "-1"
  });

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
    this.messageArrived.next({
      text: message,
      timeSent: new Date(),
    //   senderUsername: this._authService._decodedToken.unique_name,
      receiverUsername: '',
      seen: true,
    //   senderId: this._authService._decodedToken.nameid,
      recieverId: receiverId
    })
  }

  public addRecievedMessageListener = () => {
    this._hubConnection.on('receivedMessage', (message) => {
      debugger;
      console.log(message.message);
      this.messageArrived.next(message.message);

    })
  }

  public disconnect() {
    this._hubConnection.stop();
  }

  constructor(
    private _http: HttpClient,
    private _authService: AuthService
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
