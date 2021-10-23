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

  constructor(private http: HttpClient, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params: Params) => {
      this.id = params['id'];
    });
    this.activatedRoute.data.subscribe((data) => {
      console.log(data);
      this.tutorial = data;
    });
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
