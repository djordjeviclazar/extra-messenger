import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-exploretutorials',
  templateUrl: './exploretutorials.component.html',
  styleUrls: ['./exploretutorials.component.css']
})
export class ExploretutorialsComponent implements OnInit {

  _tutorialsObserver: any;
  _hotTutorialsObserver: any;

  constructor(private http: HttpClient, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this._tutorialsObserver = this.http.get<any[]>('https://localhost:5001/api/tutorial/getrecommended', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });

    this._hotTutorialsObserver = this.http.get<any[]>('https://localhost:5001/api/tutorial/gethot', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
  }

  goToTutorial(tutorialId: any) {
    this.router.navigate(['../tutorialdetails', tutorialId], { relativeTo: this.activatedRoute });
  }

  filterBeginner() {

  }

  filterIntermediate() {

  }

  filterAdvanced() {

  }

}
