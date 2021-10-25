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
import { CreatetutorialComponent } from './createtutorial/createtutorial.component';
import { TutorialdetailsComponent } from './tutorialdetails/tutorialdetails.component';
import { ProfileComponent } from './profile/profile.component';
import { BranchanalyzeComponent } from './branchanalyze/branchanalyze.component';
import { PushanalyzeComponent } from './pushanalyze/pushanalyze.component';
import { ExploretutorialsComponent } from './exploretutorials/exploretutorials.component';
//import { TutorialdetailsResolver } from './tutorialdetails/tutorialdetails.resolver';
import { StatisticsComponent } from './statistics/statistics.component';
import { IssuestatisticsComponent } from './issuestatistics/issuestatistics.component';
//import { StatisticsResolver } from './statistics/statistics.resolver';



@NgModule({
  declarations: [
    ReporouterComponent,
    FetchreposComponent,
    ReposidebarComponent,
    CreatetutorialComponent,
    TutorialdetailsComponent,
    ProfileComponent,
    BranchanalyzeComponent,
    PushanalyzeComponent,
    ExploretutorialsComponent,
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
          //{ path: 'createtutorial', component: CreatetutorialComponent, pathMatch: 'full' }
        ]
      },
      {
        path: 'fetchrepo', component: FetchreposComponent,
        children: []
      },
      {
        path: 'createtutorial', component: CreatetutorialComponent,
        children: []
      },
      {
        path: 'profile', component: ProfileComponent,
        children: []
      },
      {
        path: 'tutorialdetails/:id', component: TutorialdetailsComponent,
        //resolve: {
        //  TutorialdetailsResolver
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
        path: 'exploretutorials', component: ExploretutorialsComponent,
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
  providers: [] //TutorialdetailsResolver, StatisticsResolver
})
export class RepoModule { }
