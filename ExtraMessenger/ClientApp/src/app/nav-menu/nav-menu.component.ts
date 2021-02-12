import {Component, OnInit} from '@angular/core';
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {LoginComponent} from "../login/login.component";
import {finalize} from "rxjs/operators";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  dialogRef: MatDialogRef<LoginComponent>;

  constructor(
    private _matDialog: MatDialog
  ) {
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  openLoginDialog(): void {
    if (this.dialogRef) return;
    this.dialogRef = this._matDialog.open(LoginComponent, {
      width: '450px',
      height: '250px',
    });

    this.dialogRef.afterClosed().subscribe(() => {
      this.dialogRef = undefined;
    });
  }


  ngOnInit(): void {
  }
}
