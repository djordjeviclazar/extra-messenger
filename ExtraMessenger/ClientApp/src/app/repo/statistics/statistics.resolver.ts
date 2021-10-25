import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';

//@Injectable()
//{
//  providedIn: 'root'
//}
export class StatisticsResolver implements Resolve<any>{

  constructor(private http: HttpClient) {

  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): any {
    debugger;
    let path = 'https://localhost:5001/api/Ticket/basicstats';
    let response = this.http.get<any>(path, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
    return response;
  }
}
