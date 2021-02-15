import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  collapse = false;
  constructor() { }

  ngOnInit(): void {
  }

  toggle() {
    this.collapse = !this.collapse;
  }

}
