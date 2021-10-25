import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-reposidebar',
  templateUrl: './reposidebar.component.html',
  styleUrls: ['./reposidebar.component.css']
})
export class ReposidebarComponent implements OnInit {
  //private _router: Router

  constructor(private _router: Router) { }

  ngOnInit(): void {
  }

  navigateStatistics() {
    this._router.navigate(['repo/statistics']);
  }

  navigateExloreTicket() {
    this._router.navigate(['repo/exploretickets']);
  }

  navigateCreateTicket() {
    this._router.navigate(['repo/createticket']);
  }

  navigateFetchRepos() {
    this._router.navigate(['repo/fetchrepo']);
  }

  navigateMyProfile() {
    this._router.navigate(['repo/profile']);
  }
}
