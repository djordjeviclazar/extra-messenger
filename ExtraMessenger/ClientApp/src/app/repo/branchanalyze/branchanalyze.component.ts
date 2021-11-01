import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Params, Router } from '@angular/router';

@Component({
  selector: 'app-branchanalyze', //app-branchanalyze
  templateUrl: './branchanalyze.component.html',
  styleUrls: ['./branchanalyze.component.css']
})
export class BranchanalyzeComponent implements OnInit {

  _branchObservable: any;
  _leaningBranches: any;

  _repoId: number;
  

  constructor(private http: HttpClient, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params: Params) => this._repoId = Number(params['id']));

    this._branchObservable = this.http.get<any[]>('https://localhost:5001/api/repo/getbranches/' + this._repoId, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
  }

  analyzeBranch(branch: any) {
    this._branchObservable = this.http.get<any[]>('https://localhost:5001/api/repo/getbranches/' + this._repoId, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
  }

}
