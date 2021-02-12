import { Component, OnInit } from '@angular/core';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-message-segment',
  templateUrl: './message-segment.component.html',
  styleUrls: ['./message-segment.component.css']
})
export class MessageSegmentComponent implements OnInit {

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
  }

  send() {
    this.messageService.sendMessage(2, 'Whats UPP???')
  }

}
