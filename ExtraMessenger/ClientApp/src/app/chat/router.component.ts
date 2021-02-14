import { Component, OnDestroy, OnInit } from '@angular/core';
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
  }

  ngOnDestroy(): void {
    this._messageService.disconnect();
  }

}
