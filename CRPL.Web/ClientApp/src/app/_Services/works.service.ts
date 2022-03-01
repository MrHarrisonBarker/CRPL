import {Inject, Injectable} from '@angular/core';
import {AuthService} from "./auth.service";
import {HttpClient, HttpParams} from "@angular/common/http";
import {AlertService} from "./alert.service";
import {WorksPaths} from "../api.conts";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class WorksService
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

  public UploadWork (file: File): Observable<any>
  {
    let data = new FormData();
    data.append("file", file, file.name);
    return this.http.post(this.BaseUrl + WorksPaths.BasePath, data, {reportProgress: true, observe: 'events'});
  }

  public GetSignedWork (url: string): Observable<Blob>
  {
    return this.http.get(url, {
      reportProgress: true,
      responseType: 'blob'
    });
  }
}
