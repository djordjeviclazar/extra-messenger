import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-reporouter',
  templateUrl: './tutorialdetails.component.html',
  styleUrls: ['./tutorialdetails.component.css']
})
export class TutorialdetailsComponent implements OnInit {
  id: string;
  tutorial: any;

  constructor(private _activatedRoute: ActivatedRoute, private router: HttpClient) { }

  ngOnInit(): void {
    this._activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id');
      let path = 'https://localhost:5001/api/tutorial/gettutorial/' + this.id;
      let response = this.router.get<any>(path, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
        }
      });
      response.subscribe(x => {
        this.tutorial = x;
      });
    })
  }

  goToRepo() {
    if (this.tutorial.parts[0].repoUrl != null) {
      window.location.href = this.tutorial.parts[0].repoUrl;
    }
  }

  upvote() {

  }

  downvote() {

  }

}
