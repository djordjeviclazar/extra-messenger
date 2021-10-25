import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-reporouter',
  templateUrl: './tutorialdetails.component.html',
  styleUrls: ['./tutorialdetails.component.css']
})
export class TutorialdetailsComponent implements OnInit {
  id: string;
  tutorial: any;
  tutorialTopic: string;

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
      let path = 'https://localhost:5001/api/tutorial/gettutorial/' + this.id;
      let response = this.http.get<any>(path, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
        }
      });
      response.subscribe(x => {
        this.tutorial = x;
        this.tutorialTopic = x.topics[0];
      });

      let path2 = 'https://localhost:5001/api/tutorial/isvoted/' + this.id;
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
      this.tutorial = data;
    });*/
  }

  goToRepo() {
    if (this.tutorial.parts[0].repoUrl != null) {
      window.location.href = this.tutorial.parts[0].repoUrl;
    }
  }

  upvote() {
    this._isUp = true;
    this.tutorial.upvotes++;
    let path = 'https://localhost:5001/api/tutorial/upvote/' + this.id;
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
    this.tutorial.downvotes++;
    let path = 'https://localhost:5001/api/tutorial/downvote/' + this.id;
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
