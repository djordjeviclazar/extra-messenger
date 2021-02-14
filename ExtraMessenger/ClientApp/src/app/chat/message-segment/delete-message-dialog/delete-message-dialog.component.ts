import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MessageReturnDto } from '../../../_DTOs/messageReturnDto';

@Component({
  selector: 'app-delete-message-dialog',
  templateUrl: './delete-message-dialog.component.html',
  styleUrls: ['./delete-message-dialog.component.css']
})
export class DeleteMessageDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: MessageReturnDto) { }

  ngOnInit(): void {
  }

}
