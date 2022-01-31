import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';

import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {HomeComponent} from './home/home.component';
import {CounterComponent} from './counter/counter.component';
import {FetchDataComponent} from './fetch-data/fetch-data.component';
import {LoginButtonComponent} from './User/login-button/login-button.component';
import {AuthInterceptor} from "./_Guards/auth.interceptor";
import {AuthGuard} from "./_Guards/auth.guard";
import {ClarityModule} from "@clr/angular";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {InfoWizardComponent} from './User/info-wizard/info-wizard.component';
import {CompleteUserAndAuthGuard} from "./_Guards/complete-user-and-auth.guard";
import {LogoutButtonComponent} from './User/logout-button/logout-button.component';
import {AlertComponent} from './alert/alert.component';
import {UploadComponent} from './upload/upload.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginButtonComponent,
    InfoWizardComponent,
    LogoutButtonComponent,
    AlertComponent,
    UploadComponent
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    BrowserAnimationsModule,
    ClarityModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full'},
      {path: 'counter', component: CounterComponent, canActivate: [AuthGuard]},
      {path: 'fetch-data', canActivate: [CompleteUserAndAuthGuard], component: FetchDataComponent},
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
