import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-exploreTickets',
  templateUrl: './exploretickets.component.html',
  styleUrls: ['./exploretickets.component.css']
})
export class ExploreTicketsComponent implements OnInit {

  _TicketsObserver: any;
  _hotTicketsObserver: any;

  constructor(private http: HttpClient, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this._TicketsObserver = this.http.get<any[]>('https://localhost:5001/api/Ticket/getrecommended', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });

    this._hotTicketsObserver = this.http.get<any[]>('https://localhost:5001/api/Ticket/gethot', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
  }

  goToTicket(TicketId: any) {
    debugger;
    this.router.navigate(['../ticketdetails', TicketId], { relativeTo: this.activatedRoute });
  }

  filterBeginner() {

  }

  filterIntermediate() {

  }

  filterAdvanced() {

  }

}
