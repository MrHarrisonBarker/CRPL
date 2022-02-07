import {Inject, Injectable} from '@angular/core';
import {AuthService} from "./auth.service";
import {HttpClient} from "@angular/common/http";
import {AlertService} from "./alert.service";
import {Observable, throwError} from "rxjs";
import {CopyrightRegistrationInputModel} from "../_Models/Applications/CopyrightRegistrationInputModel";
import {CopyrightRegistrationViewModel} from "../_Models/Applications/CopyrightRegistrationViewModel";
import {FormsPaths} from "../api.conts";
import {ApplicationViewModel} from "../_Models/Applications/ApplicationViewModel";

@Injectable({
  providedIn: 'root'
})
export class FormsService
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

  public UpdateCopyrightRegistration (inputModel: CopyrightRegistrationInputModel): Observable<CopyrightRegistrationViewModel>
  {
    return this.http.post<CopyrightRegistrationViewModel>(this.BaseUrl + FormsPaths.CopyrightRegistration, inputModel);
  }

  public Cancel (id: string): Observable<any>
  {
    return throwError(new Error());
  }

  public SubmitCopyrightRegistration (id: string): Observable<CopyrightRegistrationViewModel>
  {
    return this.http.post<CopyrightRegistrationViewModel>(this.BaseUrl + FormsPaths.CopyrightRegistrationSubmit + "/" + id, null);
  }

  public GetMyApplications(): Observable<ApplicationViewModel[]>
  {
    return this.http.get<ApplicationViewModel[]>(this.BaseUrl + FormsPaths.GetMy + "/" + this.authService.UserAccount.getValue().Id);
  }

  public GetApplication(id: string): Observable<ApplicationViewModel>
  {
    return this.http.get<ApplicationViewModel>(this.BaseUrl + FormsPaths.BasePath + "/" + id);
  }
}
