import {Inject, Injectable} from '@angular/core';
import {AuthService} from "./auth.service";
import {HttpClient, HttpEvent, HttpParams, HttpRequest} from "@angular/common/http";
import {AlertService} from "./alert.service";
import {WorksPaths} from "../api.conts";
import {Observable} from "rxjs";
import {tap} from "rxjs/operators";

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

  public GetSignedWork (hash: string): Observable<Blob>
  {
    let encodedHash = hash.replace(/\+/g, '.')
    console.log(hash, encodedHash);

    return this.http.get(this.BaseUrl + WorksPaths.BasePath, {
      params: new HttpParams().set("hash", encodedHash),
      reportProgress: true,
      responseType: 'blob'
    });
  }
}
