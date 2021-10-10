import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-reporouter', //'app-fetchrepos'
  templateUrl: './fetchrepos.component.html',
  styleUrls: ['./fetchrepos.component.css']
})
export class FetchreposComponent implements OnInit {
  _jwtHelper = new JwtHelperService();
  oauth: string;
  reposObservable: any;

  constructor(private router: HttpClient) { }

  ngOnInit(): void {
    this.oauth = this._jwtHelper.decodeToken(localStorage.getItem('authToken')).userdata;
  }

  fetchrepos() {
    let response = this.router.get<boolean>('https://localhost:5000/api/user/getoauth');
    response.subscribe(isOAuth => {
      if (isOAuth) {//this.oauth != null
        this.reposObservable = this.router.get<any[]>('https://localhost:5000/api/repo/get', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
          }
        })
      }
      else {
        this.router.get<string>('https://localhost:5000/api/githubauthorize/authorize', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
          }
        }).subscribe(data => {
          window.location.href = data;
        });
      }
    });
  }

}
