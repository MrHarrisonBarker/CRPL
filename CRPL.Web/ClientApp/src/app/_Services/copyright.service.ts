import {Inject, Injectable} from '@angular/core';
import {AuthService} from "./auth.service";
import {HttpClient} from "@angular/common/http";
import {AlertService} from "./alert.service";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {CopyrightPaths, QueryPaths} from "../api.conts";
import {Observable} from "rxjs";
import {BindProposalInput, BindProposalWorkInput} from "../_Models/StructuredOwnership/BindProposalInput";
import {WarehouseService} from "./warehouse.service";
import {tap} from "rxjs/operators";

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
    private alertService: AlertService,
    private warehouse: WarehouseService)
  {
    this.BaseUrl = baseUrl;
  }

  public GetMyCopyrights (): Observable<RegisteredWorkViewModel[]>
  {
    return this.http.get<RegisteredWorkViewModel[]>(this.BaseUrl + QueryPaths.GetMy + "/" + this.authService.UserAccount.getValue().Id)
               .pipe(tap(works => this.warehouse.MyWorks = works));
  }

  public BindProposal (input: BindProposalInput): Observable<any>
  {
    return this.http.post(this.BaseUrl + CopyrightPaths.Bind, input);
  }

  public BindProposalWork (input: BindProposalWorkInput): Observable<any>
  {
    return this.http.post(this.BaseUrl + CopyrightPaths.Bind + "/work", input);
  }

  public Get (workId: string): Observable<RegisteredWorkViewModel>
  {
    return this.http.get<RegisteredWorkViewModel>(this.BaseUrl + QueryPaths.BasePath + "/" + encodeURI(workId));
  }

  public Sync (workId: string): Observable<any>
  {
    return this.http.patch(this.BaseUrl + CopyrightPaths.Sync + "/" + encodeURI(workId), null);
  }

  public Complete (id: string): Observable<any>
  {
    return this.http.patch(this.BaseUrl + CopyrightPaths.Complete + "/" + encodeURI(id), null);
  }
}
