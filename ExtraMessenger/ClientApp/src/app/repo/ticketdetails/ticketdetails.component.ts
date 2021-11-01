import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-reporouter',
  templateUrl: './ticketdetails.component.html',
  styleUrls: ['./ticketdetails.component.css']
})
export class TicketdetailsComponent implements OnInit {
  id: string;
  Ticket: any;
  TicketTopic: string;

  _isUp: boolean;
  _isDown: boolean;//votes
  upvoteBtnColor: string;
  downvoteBtnColor: string;
  //_ups: number;
  //_downs: number;

  constructor(private http: HttpClient, private router: Router, private activatedRoute: ActivatedRoute) {
    
  }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params: Params) => {
      this.id = params['id'];
      let path = 'https://localhost:5001/api/ticket/getticket/' + this.id;
      let response = this.http.get<any>(path, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
        }
      });
      response.subscribe(x => {
        this.Ticket = x;
        this.TicketTopic = x.topics[0];
      });

      let path2 = 'https://localhost:5001/api/ticket/isvoted/' + this.id;
      let response2 = this.http.get<any>(path2, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
        }
      });
      response2.subscribe(x => {
        this._isUp = x[0].upvoted;
        this._isDown = x[0].downvoted;
        /*
        this.upvoteBtnColor = this._isUp ? "accent" : "";
        this.downvoteBtnColor = this._isDown ? "accent" : "";*/
      });
    });
    
    
    /*
    this.activatedRoute.data.subscribe((data) => {
      console.log(data);
      this.Ticket = data;
    });*/
  }

  goToRepo() {
    if (this.Ticket.parts[0].repoUrl != null) {
      window.location.href = this.Ticket.parts[0].repoUrl;
    }
  }

  upvote() {
    this._isUp = true;
    this.Ticket.upvotes++;
    let path = 'https://localhost:5001/api/ticket/upvote/' + this.id;
    let response = this.http.put<any>(path, null, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
    response.subscribe(x => {
    });
    this.upvoteBtnColor = this._isUp ? "primary" : "";
  }

  downvote() {
    this._isDown = true;
    this.Ticket.downvotes++;
    let path = 'https://localhost:5001/api/ticket/downvote/' + this.id;
    let response = this.http.put<any>(path, null, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
    response.subscribe(x => {
    });
    this.downvoteBtnColor = this._isDown ? "primary" : "";
  }

}
