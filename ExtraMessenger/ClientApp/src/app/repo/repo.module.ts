import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatListItem, MatList, MatListModule } from '@angular/material/list';
//import { MatExpansionPanel, MatAccordion } from '@angular/material/expansion';
import { HttpClientModule } from '@angular/common/http';
import { FetchreposComponent } from './fetchrepos/fetchrepos.component';
import { ReporouterComponent } from './reporouter.component';
import { ReposidebarComponent } from './reposidebar/reposidebar.component';
import { CreateTicketComponent } from './createtickets/createticket.component';
import { TicketdetailsComponent } from './ticketdetails/ticketdetails.component';
import { ProfileComponent } from './profile/profile.component';
import { BranchanalyzeComponent } from './branchanalyze/branchanalyze.component';
import { PushanalyzeComponent } from './pushanalyze/pushanalyze.component';
import { ExploreTicketsComponent } from './exploretickets/exploretickets.component';
//import { TicketdetailsResolver } from './Ticketdetails/Ticketdetails.resolver';
import { StatisticsComponent } from './statistics/statistics.component';
import { IssuestatisticsComponent } from './issuestatistics/issuestatistics.component';
//import { StatisticsResolver } from './statistics/statistics.resolver';



@NgModule({
  declarations: [
    ReporouterComponent,
    FetchreposComponent,
    ReposidebarComponent,
    CreateTicketComponent,
    TicketdetailsComponent,
    ProfileComponent,
    BranchanalyzeComponent,
    PushanalyzeComponent,
    ExploreTicketsComponent,
    StatisticsComponent,
    IssuestatisticsComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild([
      
      {
        path: '', component: ReporouterComponent, pathMatch:'full',
        children: [
          //{ path: '', component: FetchreposComponent, pathMatch: 'prefix' },
          //{ path: 'fetchrepo', component: FetchreposComponent, pathMatch: 'full' },
          //{ path: 'createTicket', component: CreateTicketComponent, pathMatch: 'full' }
        ]
      },
      {
        path: 'fetchrepo', component: FetchreposComponent,
        children: []
      },
      {
        path: 'createticket', component: CreateTicketComponent,
        children: []
      },
      {
        path: 'profile', component: ProfileComponent,
        children: []
      },
      {
        path: 'ticketdetails/:id', component: TicketdetailsComponent,
        //resolve: {
        //  TicketdetailsResolver
        //},
        children: []
      },
      {
        path: 'branchanalyze/:id', component: BranchanalyzeComponent,
        children: []
      },
      {
        path: 'pushanalyze/:id', component: PushanalyzeComponent,
        children: []
      },
      {
        path: 'exploretickets', component: ExploreTicketsComponent,
        children: []
      },
      {
        path: 'statistics', component: StatisticsComponent,
        //resolve: {
        //  StatisticsResolver
        //},
        children: []
      },
      {
        path: 'issuestats/:id', component: IssuestatisticsComponent,
        children: []
      },
    ]),
    MatCardModule,
    MatButtonModule,
    MatDialogModule,
    MatDividerModule,
    MatListModule,
    //MatExpansionPanel,
    //MatAccordion,
    FormsModule,
    MatIconModule,
    ReactiveFormsModule,
    MatInputModule,
    MatSelectModule,
    MatFormFieldModule,
    HttpClientModule
  ],
  providers: [] //TicketdetailsResolver, StatisticsResolver
})
export class RepoModule { }
