import {Inject, Injectable} from '@angular/core';
import {AuthService} from "./auth.service";
import {HttpClient} from "@angular/common/http";
import {AlertService} from "./alert.service";
import {Observable, throwError} from "rxjs";
import {CopyrightRegistrationInputModel} from "../_Models/Applications/CopyrightRegistrationInputModel";
import {CopyrightRegistrationViewModel} from "../_Models/Applications/CopyrightRegistrationViewModel";
import {FormsPaths} from "../api.conts";
import {ApplicationViewModel} from "../_Models/Applications/ApplicationViewModel";
import {OwnershipRestructureViewModel} from "../_Models/Applications/OwnershipRestructureViewModel";
import {OwnershipRestructureInputModel} from "../_Models/Applications/OwnershipRestructureInputModel";
import {tap} from "rxjs/operators";
import {WarehouseService} from "./warehouse.service";

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
    private alertService: AlertService,
    private warehouse: WarehouseService)
  {
    this.BaseUrl = baseUrl;
  }

  public UpdateCopyrightRegistration (inputModel: CopyrightRegistrationInputModel): Observable<CopyrightRegistrationViewModel>
  {
    return this.http.post<CopyrightRegistrationViewModel>(this.BaseUrl + FormsPaths.CopyrightRegistration, inputModel)
               .pipe(tap(application => this.warehouse.MyApplications[this.warehouse.MyApplications.findIndex(x => x.Id == application.Id)] = application));
  }

  public Cancel (id: string): Observable<any>
  {
    return this.http.delete(this.BaseUrl + FormsPaths.Cancel + "/" + encodeURI(id));
  }

  public SubmitCopyrightRegistration (id: string): Observable<CopyrightRegistrationViewModel>
  {
    return this.http.post<CopyrightRegistrationViewModel>(this.BaseUrl + FormsPaths.CopyrightRegistrationSubmit + "/" + id, null)
               .pipe(tap(application => this.warehouse.MyApplications[this.warehouse.MyApplications.findIndex(x => x.Id == application.Id)] = application));
  }

  public GetMyApplications (): Observable<ApplicationViewModel[]>
  {
    return this.http.get<ApplicationViewModel[]>(this.BaseUrl + FormsPaths.GetMy + "/" + this.authService.UserAccount.getValue().Id)
               .pipe(tap(applications => this.warehouse.MyApplications = applications));
  }

  public GetApplication (id: string): Observable<ApplicationViewModel>
  {
    return this.http.get<ApplicationViewModel>(this.BaseUrl + FormsPaths.BasePath + "/" + id)
               .pipe(tap(application => {
                 let index = this.warehouse.MyApplications.findIndex(x => x.Id == application.Id);
                 if (index == -1) this.warehouse.MyApplications.push(application);
                 else this.warehouse.MyApplications[index] = application;
               }));
  }

  public UpdateOwnershipRestructure (inputModel: OwnershipRestructureInputModel): Observable<OwnershipRestructureViewModel>
  {
    return this.http.post<OwnershipRestructureViewModel>(this.BaseUrl + FormsPaths.OwnershipRestructure, inputModel)
               .pipe(tap(application => this.warehouse.MyApplications[this.warehouse.MyApplications.findIndex(x => x.Id == application.Id)] = application));
  }

  public SubmitOwnershipRestructure (id: string) : Observable<OwnershipRestructureViewModel>
  {
    return this.http.post<OwnershipRestructureViewModel>(this.BaseUrl + FormsPaths.OwnershipRestructureSubmit + "/" + id, null)
               .pipe(tap(application => this.warehouse.MyApplications[this.warehouse.MyApplications.findIndex(x => x.Id == application.Id)] = application));
  }
}
