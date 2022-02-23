import {Inject, Injectable} from '@angular/core';
import {AuthService} from "./auth.service";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {AlertService} from "./alert.service";
import {WarehouseService} from "./warehouse.service";
import {Observable} from "rxjs";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {QueryPaths} from "../api.conts";
import {Sortable, StructuredQuery} from "../_Models/StructuredQuery/StructuredQuery";

@Injectable({
  providedIn: 'root'
})
export class QueryService
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

  public GetRecent (): Observable<RegisteredWorkViewModel[]>
  {
    let query: StructuredQuery = {
      SortBy: Sortable.Created
    }

    return this.http.post<RegisteredWorkViewModel[]>(this.BaseUrl + QueryPaths.Search, query, {
      headers: new HttpHeaders().set('from', '0').set('take', '100')
    });
  }

  public Search ()
  {

  }
}
