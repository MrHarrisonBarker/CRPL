import {Inject, Injectable} from '@angular/core';
import {AuthService} from "./auth.service";
import {HttpClient, HttpHeaders, HttpParams} from "@angular/common/http";
import {AlertService} from "./alert.service";
import {WarehouseService} from "./warehouse.service";
import {Observable} from "rxjs";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {QueryPaths} from "../api.conts";
import {Sortable, StructuredQuery} from "../_Models/StructuredQuery/StructuredQuery";
import {DisputeViewModel} from "../_Models/Applications/DisputeViewModel";

@Injectable({
  providedIn: 'root'
})
export class QueryService
{
  private readonly pageWidth = 100;
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

  public GetRecent (page: number): Observable<RegisteredWorkViewModel[]>
  {
    let query: StructuredQuery = {
      SortBy: Sortable.Created
    }

    return this.http.post<RegisteredWorkViewModel[]>(this.BaseUrl + QueryPaths.Search, query, {
      params: new HttpParams().set('from', page * this.pageWidth).set('take', this.pageWidth)
    });
  }

  public Search (page: number, query: StructuredQuery): Observable<RegisteredWorkViewModel[]>
  {
    return this.http.post<RegisteredWorkViewModel[]>(this.BaseUrl + QueryPaths.Search, query, {
      params: new HttpParams().set('from', page * this.pageWidth).set('take', this.pageWidth)
    });
  }

  public GetDisputes(page: number): Observable<DisputeViewModel[]>
  {
    return this.http.get<DisputeViewModel[]>(this.BaseUrl + QueryPaths.Disputes, {
      params: new HttpParams().set('from', page * this.pageWidth).set('take', this.pageWidth)
    })
  }
}
