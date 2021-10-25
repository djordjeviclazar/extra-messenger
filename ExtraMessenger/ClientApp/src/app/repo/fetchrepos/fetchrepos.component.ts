import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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
  oldReposObservable: any;

  constructor(private http: HttpClient, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.oauth = this._jwtHelper.decodeToken(localStorage.getItem('authToken')).userdata;
    this.oldReposObservable = this.http.get<any[]>('https://localhost:5001/api/repo/getrepos', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
  }

  goToBranches(repoId: any) {
    this.router.navigate(['../branchanalyze', repoId], { relativeTo: this.activatedRoute });
  }

  goToPushes(repoId: any) {
    this.router.navigate(['../pushanalyze', repoId], { relativeTo: this.activatedRoute });
  }

  fetchRepoInfo(id: any) {
    /*
    let response = this.http.get<boolean>('https://localhost:5001/api/repo/fetchrepoinfo/' + id, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
    response.subscribe(x => console.log(x));
    */
    this.router.navigate(['../issuestats', id], { relativeTo: this.activatedRoute });
  }

  fetchrepos() {
    let response = this.http.get<boolean>('https://localhost:5001/api/githubauthorize/getoauth', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
    response.subscribe(isOAuth => {
      if (isOAuth) {//this.oauth != null
        this.reposObservable = this.http.get<any[]>('https://localhost:5001/api/repo/fetchrepos', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
          }
        });
        this.oldReposObservable = this.http.get<any[]>('https://localhost:5001/api/repo/getrepos', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
          }
        });
      }
      else {
        this.http.post<string>('https://localhost:5001/api/githubauthorize/authorize', null, {
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
