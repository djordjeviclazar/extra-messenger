import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';


@Component({
  selector: 'app-reporouter', //'app-createTicket'
  templateUrl: './createticket.component.html',
  styleUrls: ['./createticket.component.css']
})
export class CreateTicketComponent implements OnInit {

  isFilledRequired: boolean = false;
  title: string = '';
  intro: string = '';
  selectedDifficulty: string = '';
  selectedTopic: string = '';
  selectedRepo: any = {};
  difficulties: any = this.router.get<any[]>('https://localhost:5001/api/Ticket/getdifficulties', {
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
    }
  });
  topics: any = this.router.get<any[]>('https://localhost:5001/api/Ticket/gettopics', {
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
    }
  });
  repos: any = this.router.get<any[]>('https://localhost:5001/api/repo/getrepos', {
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
    }
  });


  constructor(private router: HttpClient
    , private _alertifyService: AlertifyService
    , private _activatedRoute: ActivatedRoute
    , private _routeNavigator: Router) {

  }

  ngOnInit(): void {

  }

  onEnterTitle() {
    if (this.title !== '') {
      this.isFilledRequired = true;
    }
    else {
      this.isFilledRequired = false;
    }
  }

  createTicket() {
    let TicketDto: any = {};
    let part: any = {};
    let partList: any[] = [];
    let selectedTopics: string[] = [];

    TicketDto.Title = this.title;
    TicketDto.Introduction = this.intro;
    TicketDto.Difficulty = this.selectedDifficulty;

    selectedTopics.push(this.selectedTopic);
    TicketDto.Topics = selectedTopics;

    part.Title = '';
    part.RepoId = this.selectedRepo.id;
    part.RepoUrl = this.selectedRepo.repoUrl;
    part.Description = '';
    partList.push(part);
    TicketDto.Parts = partList;

    const path = 'https://localhost:5001/api/Ticket/create';
    this.router.post<any>(path, TicketDto, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    }).subscribe((response) => {
      if (response.success) {
        this._alertifyService.success("Ticket created");
        const pathToTicket = 'https://localhost:4200/repo/Ticketdetails/'; //https://localhost:4200/repo/Ticketdetails/
        this._routeNavigator.navigate([pathToTicket, response.id]);
      }
      else {
        this._alertifyService.error("Error: " + response.message);
      }
    })
  }
}
