import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-reporouter', //'app-createtutorial'
  templateUrl: './createtutorial.component.html',
  styleUrls: ['./createtutorial.component.css']
})
export class CreatetutorialComponent implements OnInit {

  isFilledRequired: boolean = false;
  title: string = '';
  intro: string = '';
  selectedDifficulty: string = '';
  selectedTopic: string = '';
  selectedRepo: any = {};
  difficulties: any = this.router.get<any[]>('https://localhost:5001/api/tutorial/getdifficulties', {
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
    }
  });
  topics: any = this.router.get<any[]>('https://localhost:5001/api/tutorial/gettopics', {
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
    }
  });
  repos: any = this.router.get<any[]>('https://localhost:5001/api/repo/getrepos', {
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
    }
  });


  constructor(private router: HttpClient, private _alertifyService: AlertifyService) { }

  ngOnInit(): void {

  }

  onEnterTitle() {
    if (this.title !== '') {
      this.isFilledRequired = true;
    }
    else {
      this.isFilledRequired = false;
    }
  }

  createTutorial() {
    let tutorialDto: any = {};
    let part: any = {};
    let partList: any[] = [];
    let selectedTopics: string[] = [];

    tutorialDto.Title = this.title;
    tutorialDto.Introduction = this.intro;
    tutorialDto.Difficulty = this.selectedDifficulty;

    selectedTopics.push(this.selectedTopic);
    tutorialDto.Topics = selectedTopics;
    
    part.Title = '';
    part.RepoId = this.selectedRepo.id;
    part.RepoUrl = this.selectedRepo.HtmlUrl;
    part.Description = '';
    partList.push(part);
    tutorialDto.Parts = partList;

    const path = 'https://localhost:5001/api/tutorial/create';
    this.router.post<any>(path, tutorialDto, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    }).subscribe((response) => {
      if (response.success) {
        this._alertifyService.success("Tutorial created");
      }
      else {
        this._alertifyService.error("Error: " + response.message);
      }
    })
  }
}
