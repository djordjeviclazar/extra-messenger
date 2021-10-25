import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';

//@Injectable()
export class TicketdetailsResolver implements Resolve<any>{

  constructor(
    private http: HttpClient
  ) {

  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {
    let path = 'https://localhost:5001/api/ticket/getticket/' + route.params['id'];
    let response = this.http.get<any>(path, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
    return response;
  }
}
