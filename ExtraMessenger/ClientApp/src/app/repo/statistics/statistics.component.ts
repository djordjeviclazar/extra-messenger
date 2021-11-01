import { Component, OnInit, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
//import { ChartOptions, ChartType } from 'chart.js';
import * as Chart from 'chart.js';
import { Label } from 'ng2-charts';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
//import * as pluginDataLabels from 'chartjs-plugin-datalabels';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.css']
})
export class StatisticsComponent implements OnInit {
  labelArray: any[];
  repositoryArray: any[];
  TicketArray: any[];

  @ViewChild('barCanvas') private barCanvas: ElementRef;
  barChart: any;

  constructor(private _activatedRoute: ActivatedRoute, private http: HttpClient) {
    //}
    //debugger;
    //this._activatedRoute.data.pipe(map(data => {
    //  debugger;
    //  this.repositoryArray = data.repoArray;
    //  this.TicketArray = data.TicketArray;
    //  this.labelArray = data.labels;
    //})).subscribe(x => {
    //  this.barChartMethod();
    //});
  }

  ngOnInit(): void {
    let path = 'https://localhost:5001/api/Ticket/basicstats';
    let response = this.http.get<any>(path, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
      }
    });
    response.subscribe(x => {
      this.repositoryArray = x.repoArray;
      this.TicketArray = x.ticketArray;
      this.labelArray = x.labels;

      this.barChartMethod();
    })
  }

  ngAfterViewInit(): void {
    //this.barChartMethod();
  }

  barChartMethod() {

    let backgroundColor = 'rgba(255, 99, 132, 0.2)';
    let borderColor = 'rgba(255,99,132,1)';
    let bckColorArray = [], borderColorArray = [], bckColorArray2 = [], borderColorArray2 = [];
    bckColorArray = this.repositoryArray.map((val, ind, a) => backgroundColor);
    borderColorArray = this.repositoryArray.map((val, ind, a) => backgroundColor);
    bckColorArray2 = this.repositoryArray.map((val, ind, a) => 'rgba(54, 162, 235, 0.2)');
    borderColorArray2 = this.repositoryArray.map((val, ind, a) => 'rgba(54, 162, 235, 1)');

    this.barChart = new Chart(this.barCanvas.nativeElement, {
      type: 'radar',
      data: {
        labels: this.labelArray,
        datasets: [{
          label: '# of Repositories',
          data: this.repositoryArray,
          backgroundColor: bckColorArray,
          borderColor: borderColorArray,
          borderWidth: 1
        },
        {
          label: '# of Upvoted Tickets',
          data: this.TicketArray,
          backgroundColor: bckColorArray2,
          borderColor: borderColorArray2,
          borderWidth: 1
          },
        //  {
        //    label: '# of Downvoted Tickets',
        //    data: ,
        //    backgroundColor: bckColorArray,
        //    borderColor: borderColorArray,
        //    borderWidth: 1
        //  },
        ]
      },
      //options: {
      //  scales: {
      //    yAxes: [{
      //      ticks: {
      //        beginAtZero: true
      //      }
      //    }]
      //  }
      //}


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
