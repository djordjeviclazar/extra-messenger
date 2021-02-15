import { Component, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-router',
  templateUrl: './router.component.html',
  styleUrls: ['./router.component.css']
})
export class RouterComponent implements OnInit, OnDestroy {

  constructor(private _messageService: MessageService) { }

  ngOnInit(): void {
    this._messageService.startConnection();
    this._messageService.addRecievedMessageListener();
    this._messageService.messageArrived = new BehaviorSubject<any>(null);
    this._messageService.messageThread = new BehaviorSubject<any>(null);
  }

  ngOnDestroy(): void {
    this._messageService.disconnect();
  }

}
