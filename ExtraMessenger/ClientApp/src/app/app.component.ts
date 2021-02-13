import { Component, OnDestroy, OnInit } from '@angular/core';
import { MessageService } from './_services/message.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'app';

  constructor(
    private _messageService: MessageService
  ) {

  }

  ngOnInit(): void {
    this._messageService.startConnection();
    this._messageService.addRecievedMessageListener();
  }

  ngOnDestroy(): void {
    this._messageService.disconnect();
  }

}
