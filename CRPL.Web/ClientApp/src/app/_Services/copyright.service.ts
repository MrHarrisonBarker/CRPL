import {Inject, Injectable} from '@angular/core';
import {AuthService} from "./auth.service";
import {HttpClient, HttpEvent} from "@angular/common/http";
import {AlertService} from "./alert.service";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {CopyrightPaths} from "../api.conts";
import {Observable} from "rxjs";
import {BindProposalInput} from "../_Models/StructuredOwnership/BindProposalInput";

@Injectable({
  providedIn: 'root'
})
export class CopyrightService
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

  public GetMyCopyrights (): Observable<RegisteredWorkViewModel[]>
  {
    return this.http.get<RegisteredWorkViewModel[]>(this.BaseUrl + CopyrightPaths.GetMy + "/" + this.authService.UserAccount.getValue().Id);
  }

  public BindProposal (input: BindProposalInput): Observable<any>
  {
    return this.http.post(this.BaseUrl + CopyrightPaths.Bind, input);
  }
}
