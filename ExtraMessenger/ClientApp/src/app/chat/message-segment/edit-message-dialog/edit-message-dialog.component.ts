import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MessageReturnDto } from '../../../_DTOs/messageReturnDto';

@Component({
  selector: 'app-edit-message-dialog',
  templateUrl: './edit-message-dialog.component.html',
  styleUrls: ['./edit-message-dialog.component.css']
})
export class EditMessageDialogComponent implements OnInit {

  editMessageForm: FormGroup;
  
  constructor(
    private _formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: MessageReturnDto,
    public dialogRef: MatDialogRef<EditMessageDialogComponent>,
  ) { }

  ngOnInit(): void {
    this.editMessageForm = this._formBuilder.group({
      message: [this.data.content, Validators.required]
    });
  }

  finishEditing() {
    if (this.editMessageForm.invalid)
      return;

    const editedMessage = Object.assign<MessageReturnDto, MessageReturnDto>({}, this.data);
    editedMessage.content = this.editMessageForm.get('message').value;
    this.dialogRef.close(editedMessage);
  }

  cancel() {
    this.dialogRef.close(this.data);
  }
}
