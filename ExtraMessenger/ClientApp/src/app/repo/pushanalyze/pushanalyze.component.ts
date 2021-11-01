import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Params, Router } from '@angular/router';

@Component({
  selector: 'app-pushanalyze',
  templateUrl: './pushanalyze.component.html',
  styleUrls: ['./pushanalyze.component.css']
})
export class PushanalyzeComponent implements OnInit {

  _pushObservable: any;
  _affectedBranches: any

  _repoId: number;

  constructor(private http: HttpClient, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params: Params) => this._repoId = Number(params['id']));

    this._pushObservable = this.http.get<any[]>('https://localhost:5001/api/repo/getpushes/' + this._repoId, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
  }

}
