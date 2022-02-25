import {BrowserModule} from '@angular/platform-browser';
import {ErrorHandler, NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';

import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {HomeComponent} from './home/home.component';
import {LoginButtonComponent} from './User/login-button/login-button.component';
import {AuthInterceptor} from "./_Guards/auth.interceptor";
import {AuthGuard} from "./_Guards/auth.guard";
import {ClarityModule} from "@clr/angular";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {InfoWizardComponent} from './User/info-wizard/info-wizard.component';
import {AlertComponent} from './alert/alert.component';
import {UploadComponent} from './upload/upload.component';
import {ApplicationsComponent} from './Forms/applications/applications.component';
import {DashboardComponent} from './Dashboard/dashboard/dashboard.component';
import {ApplicationsViewComponent} from './Dashboard/applications-view/applications-view.component';
import {ApplicationStatusComponent} from './Core/application-status/application-status.component';
import {CopyrightStatusComponent} from './Core/copyright-status/copyright-status.component';
import {OwnershipStructureViewComponent} from './Core/ownership-structure-view/ownership-structure-view.component';
import {CpyRestructureViewComponent} from './Dashboard/cpy-restructure-view/cpy-restructure-view.component';
import {CpyRestructureFormComponent} from "./Forms/cpy-restructure-form/cpy-restructure-form.component";
import {CpyRegistrationFormComponent} from "./Forms/cpy-registration-form/cpy-registration-form.component";
import {OwnershipStakeFormComponent} from "./Forms/ownership-stake-form/ownership-stake-form.component";
import {CopyrightViewComponent} from "./Dashboard/copyright-view/copyright-view.component";
import {
  SubmittedViewComponent
} from "./Dashboard/cpy-registration-view/cpy-registration-view-submitted/cpy-registration-view-submitted.component";
import {IndervidualApplicationComponent} from "./Forms/application-indervidual/application.component";
import {OwnershipStructureFormComponent} from "./Forms/ownership-structure-form/ownership-structure-form.component";
import {CpyRegistrationViewComponent} from "./Dashboard/cpy-registration-view/cpy-registration-view.component";
import {ApplicationTypeComponent} from "./Core/application-type/application-type.component";
import {
  CpyRestructureViewSubmittedComponent
} from "./Dashboard/cpy-restructure-view/cpy-restructure-view-submitted/cpy-restructure-view-submitted.component";
import {CopyrightComponent} from './copyright/copyright.component';
import {HttpLoadingInterceptor} from "./Core/HttpLoadingInterceptor";
import {BindStatusComponent} from './Core/bind-status/bind-status.component';
import {CompleteUserAndAuthGuard} from "./_Guards/complete-user-and-auth.guard";
import {
  CpyRegistrationViewFailedComponent
} from './Dashboard/cpy-registration-view/cpy-registration-view-failed/cpy-registration-view-failed.component';
import {
  CpyRestructureViewFailedComponent
} from "./Dashboard/cpy-restructure-view/cpy-restructure-view-failed/cpy-restructure-view-failed.component";
import {
  CopyrightViewExpiredComponent
} from './Dashboard/copyright-view/copyright-view-expired/copyright-view-expired.component';
import {WorkApplicationsComponent} from './Core/work-applications/work-applications.component';
import {DisputeFormComponent} from './Forms/dispute-form/dispute-form.component';
import {RememberedGuard} from "./_Guards/remembered.guard";
import { CopyrightMinComponent } from './copyright/copyright-min/copyright-min.component';
import { DisputeViewCompletedComponent } from './Dashboard/dispute-view/dispute-view-completed/dispute-view-completed.component';
import { DisputeViewSubmittedComponent } from './Dashboard/dispute-view/dispute-view-submitted/dispute-view-submitted.component';
import { UserCardComponent } from './Core/user-card/user-card.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    LoginButtonComponent,
    InfoWizardComponent,
    AlertComponent,
    UploadComponent,
    ApplicationsComponent,
    CpyRestructureFormComponent,
    CpyRegistrationFormComponent,
    OwnershipStructureViewComponent,
    OwnershipStakeFormComponent,
    OwnershipStakeFormComponent,
    DashboardComponent,
    CopyrightViewComponent,
    SubmittedViewComponent,
    ApplicationsViewComponent,
    ApplicationStatusComponent,
    CopyrightStatusComponent,
    OwnershipStructureViewComponent,
    CpyRestructureViewComponent,
    IndervidualApplicationComponent,
    OwnershipStructureFormComponent,
    CpyRegistrationViewComponent,
    ApplicationTypeComponent,
    CpyRestructureViewSubmittedComponent,
    CopyrightComponent,
    BindStatusComponent,
    CpyRegistrationViewFailedComponent,
    CpyRestructureViewFailedComponent,
    CopyrightViewExpiredComponent,
    WorkApplicationsComponent,
    DisputeFormComponent,
    CopyrightMinComponent,
    DisputeViewCompletedComponent,
    DisputeViewSubmittedComponent,
    UserCardComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    BrowserAnimationsModule,
    ClarityModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full', canActivate: [RememberedGuard]},
      {path: 'apps', component: ApplicationsComponent, canActivate: [RememberedGuard, CompleteUserAndAuthGuard]},
      {path: 'user/info', component: InfoWizardComponent, canActivate: [RememberedGuard, AuthGuard]},
      {
        path: 'application/:id',
        component: IndervidualApplicationComponent,
        canActivate: [RememberedGuard, CompleteUserAndAuthGuard]
      },
      {path: 'dashboard', component: DashboardComponent, canActivate: [RememberedGuard, CompleteUserAndAuthGuard]},
      {
        path: 'register',
        component: CpyRegistrationFormComponent,
        canActivate: [RememberedGuard, CompleteUserAndAuthGuard]
      },
      {path: 'cpy/:id', component: CopyrightComponent, canActivate: [RememberedGuard]},
    ]),
    ReactiveFormsModule
  ],
  providers: [
    {provide: ErrorHandler},
    {provide: HTTP_INTERCEPTORS, useClass: HttpLoadingInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule
{
}
