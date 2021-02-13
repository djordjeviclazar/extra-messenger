import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class MessageService {
  _baseUrl = 'http://localhost:5000/';
  _message: string = '';
  _hubConnection: signalR.HubConnection;
  _receivers: any[] = [];
  messageThread = new BehaviorSubject<{ recieverId: number, chatInteractionId: number, senderName: string }>({
    recieverId: -2,
    chatInteractionId: -2,
    senderName: ''
  });
  messageArrived = new BehaviorSubject<any>({
    text: '',
    timeSent: new Date(),
    senderUsername: '',
    receiverUsername: '',
    seen: true,
    senderId: -1,
    recieverId: -1
  });

  public startConnection = () => {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this._baseUrl + 'chat', { accessTokenFactory: () => localStorage.getItem('superTouristToken') })
      .build();

    this._hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ', err));
  }

  public sendMessage = (receiverId: number, message: any) => {
    if (receiverId == null) { return; }
    let msgObject = { message: message };

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
    this._hubConnection.on('recievedMessage', (message) => {
      console.log(message)
      this.messageArrived.next(message);

    })
  }

  public disconnect() {
    this._hubConnection.stop();
  }

  constructor(private _http: HttpClient) { }

  setReceiveMessageHandler(receiveMessageHandler: any, userId: number) {
    let newReceiver = { id: userId, handler: receiveMessageHandler };
    this._receivers.push(newReceiver);
  }

  getContacts(): import("rxjs").Observable <any[]> { //import("../_models/chatInteraction").
    return this._http.get<any[]>('http://localhost:5000/' + 'message/contacts');
  }

  getMessages(): Observable<any[]> {
    if (this.messageThread.value == undefined ||
      this.messageThread.value.recieverId == undefined ||
      this.messageThread.value.chatInteractionId == undefined)
      return;

    return this._http.get<any[]>('http://localhost:5000/' + 'message/' + this.messageThread.value.chatInteractionId);
  }

}
