import {Inject, Injectable} from '@angular/core';
import {Observable, of, throwError} from "rxjs";
import {UserAccountStatusModel} from "../_Models/Account/UserAccountStatusModel";
import {HttpClient, HttpParams} from "@angular/common/http";
import {UserPaths} from "../api.conts";
import {AuthService} from "./auth.service";
import {AccountInputModel} from "../_Models/Account/AccountInputModel";
import {catchError, tap} from "rxjs/operators";
import {AlertService} from "./alert.service";
import {UserAccountMinimalViewModel} from "../_Models/Account/UserAccountMinimalViewModel";

@Injectable({
  providedIn: 'root'
})
export class UserService
{
  private readonly BaseUrl: string;

  constructor (
    private authService: AuthService,
    private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string,
    private alertService: AlertService)
  {
    this.BaseUrl = baseUrl;
  }

  // Guid accountId, AccountInputModel accountInputModel
  public UpdateAccount (accountInput: AccountInputModel): Observable<UserAccountStatusModel>
  {
    if (!this.authService.IsAuthenticated.value) return throwError(new Error("The user is not authenticated!"));

    return this.http.post<UserAccountStatusModel>(this.BaseUrl + UserPaths.Account, accountInput, {
      params: new HttpParams().set('accountId', this.authService.UserAccount.value.Id)
    }).pipe(tap(status =>
    {
      console.log("updated user account", status);
      this.authService.UserAccount.next(status.UserAccount);
    }));
  }

  public IsPhoneUnique (phone: string): Observable<boolean>
  {
    return this.http.get<boolean>(this.BaseUrl + UserPaths.PhoneExists, {params: new HttpParams().set('phone', phone)});
  }

  public IsEmailUnique (email: string): Observable<boolean>
  {
    return this.http.get<boolean>(this.BaseUrl + UserPaths.EmailExists, {params: new HttpParams().set('email', email)});
  }

  public Search(address: string) : Observable<UserAccountMinimalViewModel[]>
  {
    return this.http.get<UserAccountMinimalViewModel[]>(this.BaseUrl + UserPaths.Search + "/" + encodeURI(address));
  }
}
