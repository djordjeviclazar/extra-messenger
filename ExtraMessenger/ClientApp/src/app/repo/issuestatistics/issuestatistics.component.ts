import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import * as Chart from 'chart.js';

@Component({
  selector: 'app-issuestatistics',
  templateUrl: './issuestatistics.component.html',
  styleUrls: ['./issuestatistics.component.css']
})
export class IssuestatisticsComponent implements OnInit {
  id: string;

  labelArray: any[];
  issueStats: any[];
  barIssueStats: any[];
  barLabelArray: any[];

  @ViewChild('pieCanvas') private pieCanvas: ElementRef;
  @ViewChild('barCanvas') private barCanvas: ElementRef;
  barChart: any;
  pieChart: any;

  constructor(private http: HttpClient, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params: Params) => {
      
      this.id = params['id'];
      let path = 'https://localhost:5001/api/repo/fetchrepoinfo/' + this.id;
      let response = this.http.get<any>(path, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
        }
      });
      response.subscribe(x => {
        
      });
    });
    
  }

  pieChartMethod() {

    this.pieChart = new Chart(this.pieCanvas.nativeElement, {
      type: 'pie',
      data: {
        labels: this.labelArray,
        datasets: [{
          label: '# closed/opened/reopend issues',
          data: this.issueStats,
          backgroundColor: [
            'rgba(255, 99, 132, 0.2)',
            'rgba(54, 162, 235, 0.2)',
            'rgba(255, 206, 86, 0.2)'
          ],
          borderColor: [
            'rgba(255,99,132,1)',
            'rgba(54, 162, 235, 1)',
            'rgba(255, 206, 86, 1)'
          ],
          borderWidth: 1
        },
        ]
      },
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        }
      }


      //'rgba(255, 99, 132, 0.2)',
      //'rgba(54, 162, 235, 0.2)',
      //'rgba(255, 206, 86, 0.2)',
      //'rgba(75, 192, 192, 0.2)',
      //'rgba(153, 102, 255, 0.2)',
      //'rgba(255, 159, 64, 0.2)'

      //'rgba(255,99,132,1)',
      //'rgba(54, 162, 235, 1)',
      //'rgba(255, 206, 86, 1)',
      //'rgba(75, 192, 192, 1)',
      //'rgba(153, 102, 255, 1)',
      //'rgba(255, 159, 64, 1)'
    });
  }

  ngAfterViewInit(): void {
    this.labelArray = this.barLabelArray = ['NN', 'NN', 'NN'];
    this.issueStats = this.barIssueStats = [15, 15, 15];
    this.pieChartMethod();
    this.barChartMethod();
  }

  barChartMethod() {

    this.barChart = new Chart(this.barCanvas.nativeElement, {
      type: 'bar',
      data: {
        labels: this.barLabelArray,
        datasets: [{
          label: '# issue in state',
          data: this.barIssueStats,
          backgroundColor: [
            'rgba(255, 99, 132, 0.2)',
            'rgba(54, 162, 235, 0.2)',
            'rgba(255, 206, 86, 0.2)'
          ],
          borderColor: [
            'rgba(255,99,132,1)',
            'rgba(54, 162, 235, 1)',
            'rgba(255, 206, 86, 1)'
          ],
          borderWidth: 1
        },
        ]
      },
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        }
      }


      //'rgba(255, 99, 132, 0.2)',
      //'rgba(54, 162, 235, 0.2)',
      //'rgba(255, 206, 86, 0.2)',
      //'rgba(75, 192, 192, 0.2)',
      //'rgba(153, 102, 255, 0.2)',
      //'rgba(255, 159, 64, 0.2)'

      //'rgba(255,99,132,1)',
      //'rgba(54, 162, 235, 1)',
      //'rgba(255, 206, 86, 1)',
      //'rgba(75, 192, 192, 1)',
      //'rgba(153, 102, 255, 1)',
      //'rgba(255, 159, 64, 1)'
    });
  }

}
