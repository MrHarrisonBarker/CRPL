import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
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
import {LogoutButtonComponent} from './User/logout-button/logout-button.component';
import {AlertComponent} from './alert/alert.component';
import {UploadComponent} from './upload/upload.component';
import {ApplicationsComponent} from './Forms/applications/applications.component';
import { DashboardComponent } from './Dashboard/dashboard/dashboard.component';
import { ApplicationsViewComponent } from './Dashboard/applications-view/applications-view.component';
import { ApplicationStatusComponent } from './Core/application-status/application-status.component';
import { CopyrightStatusComponent } from './Core/copyright-status/copyright-status.component';
import { OwnershipStructureViewComponent } from './Core/ownership-structure-view/ownership-structure-view.component';
import { CpyRestructureViewComponent } from './Dashboard/cpy-restructure-view/cpy-restructure-view.component';
import {CpyRestructureFormComponent} from "./Forms/cpy-restructure-form/cpy-restructure-form.component";
import {CpyRegistrationFormComponent} from "./Forms/cpy-registration-form/cpy-registration-form.component";
import {OwnershipStakeFormComponent} from "./Forms/ownership-stake-form/ownership-stake-form.component";
import {CopyrightViewComponent} from "./Dashboard/copyright-view/copyright-view.component";
import {SubmittedViewComponent} from "./Dashboard/cpy-registration-view/cpy-registration-view-submitted/cpy-registration-view-submitted.component";
import {IndervidualApplicationComponent} from "./Forms/application-indervidual/application.component";
import {OwnershipStructureFormComponent} from "./Forms/ownership-structure-form/ownership-structure-form.component";
import {CpyRegistrationViewComponent} from "./Dashboard/cpy-registration-view/cpy-registration-view.component";
import {ApplicationTypeComponent} from "./Core/application-type/application-type.component";
import {CpyRestructureViewSubmittedComponent} from "./Dashboard/cpy-restructure-view/cpy-restructure-view-submitted/cpy-restructure-view-submitted.component";
import { CopyrightComponent } from './copyright/copyright.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    LoginButtonComponent,
    InfoWizardComponent,
    LogoutButtonComponent,
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
    CopyrightComponent
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    BrowserAnimationsModule,
    ClarityModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full'},
      {path: 'apps', component: ApplicationsComponent, canActivate: [AuthGuard]},
      {path: 'user/info', canActivate: [AuthGuard], component: InfoWizardComponent},
      {path: 'application/:id', canActivate: [AuthGuard], component: IndervidualApplicationComponent},
      {path: 'dashboard', canActivate: [AuthGuard], component: DashboardComponent},
      {path: 'register', canActivate: [AuthGuard], component: CpyRegistrationFormComponent},
      {path: 'cpy/:id', component: CopyrightComponent},
    ]),
    ReactiveFormsModule
  ],
  providers: [{provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true}],
  bootstrap: [AppComponent]
})
export class AppModule
{
}
