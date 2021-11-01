import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  username: any = '';
  rating: any = '';
  _interests: any[];
  _othertopics: any[];
  _profile: any;
  _imageUrl: string;
  _Tickets: any;

  constructor(private router: HttpClient) { }

  ngOnInit(): void {
    let user = Math.floor(Math.random() * 100) + 1 > 50 ? 'men' : 'women';
    this._imageUrl = `https://randomuser.me/api/portraits/${user}/${Math.floor(Math.random() * 50) + 1}.jpg`;

    this.router.get<any>('https://localhost:5001/api/user/getprofile', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    }).subscribe(x => {
      this.username = x.username;
      this.rating = x.rating;
      this._interests = x.likedTopics;
      this._othertopics = x.otherTopics;
    });

    this._Tickets = this.router.get<any[]>('https://localhost:5001/api/user/gettopTickets', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
  }

  addInterest(name: any) {
    this.router.get<any>('https://localhost:5001/api/user/addinterest/' + name, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    }).subscribe(x => {
      if (x.success) {
        this._othertopics = this._othertopics.filter((val, ind, arr) => val !== name);
        this._interests.push(name);
      }
    });
  }

  removeInterest(name: any) {
    this.router.get<any>('https://localhost:5001/api/user/removeinterest/' + name, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    }).subscribe(x => {
      if (x.success) {
        this._interests = this._interests.filter((val, ind, arr) => val !== name);
        this._othertopics.push(name);
      }
    });
  }

}
