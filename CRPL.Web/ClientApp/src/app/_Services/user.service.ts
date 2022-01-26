import {Inject, Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {UserAccountStatusModel} from "../_Models/Account/UserAccountStatusModel";
import {HttpClient, HttpParams} from "@angular/common/http";
import {UserPaths} from "../api.conts";
import {AuthService} from "./auth.service";
import {AccountInputModel} from "../_Models/Account/AccountInputModel";
import {tap} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class UserService
{
  private readonly BaseUrl: string;

  constructor (private authService: AuthService, private http: HttpClient, @Inject('BASE_URL') baseUrl: string)
  {
    this.BaseUrl = baseUrl;
  }

  // Guid accountId, AccountInputModel accountInputModel
  public UpdateAccount (accountInput: AccountInputModel): Observable<UserAccountStatusModel>
  {
    if (!this.authService.IsAuthenticated.value) throw new Error("The user is not authenticated!");
    return this.http.post<UserAccountStatusModel>(this.BaseUrl + UserPaths.Account, accountInput, {
      params: new HttpParams().set('accountId', this.authService.UserAccount.value.Id)
    }).pipe(tap(status =>
    {
      console.log("updated user account", status);
      this.authService.UserAccount.next(status.UserAccount);
    }));
  }
}
