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
import {CpyRegistrationComponent} from './Forms/cpy-registration/cpy-registration.component';
import {CpyRestructureComponent} from './Forms/cpy-restructure/cpy-restructure.component';
import {OwnershipStakeComponent} from './Forms/ownership-stake/ownership-stake.component';
import { OwnershipStructureComponent } from './Forms/ownership-structure/ownership-structure.component';

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
    CpyRegistrationComponent,
    CpyRestructureComponent,
    OwnershipStakeComponent,
    OwnershipStructureComponent
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
      // {path: 'counter', canActivate: [AuthGuard]},
      // {path: 'fetch-data', canActivate: [CompleteUserAndAuthGuard]},
      {path: 'user/info', canActivate: [AuthGuard], component: InfoWizardComponent}
    ]),
    ReactiveFormsModule
  ],
  providers: [{provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true}],
  bootstrap: [AppComponent]
})
export class AppModule
{
}
