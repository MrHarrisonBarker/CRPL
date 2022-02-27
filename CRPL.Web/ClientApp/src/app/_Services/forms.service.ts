import {Inject, Injectable} from '@angular/core';
import {AuthService} from "./auth.service";
import {HttpClient, HttpParams, HttpResponse} from "@angular/common/http";
import {AlertService} from "./alert.service";
import {Observable} from "rxjs";
import {CopyrightRegistrationInputModel} from "../_Models/Applications/CopyrightRegistrationInputModel";
import {CopyrightRegistrationViewModel} from "../_Models/Applications/CopyrightRegistrationViewModel";
import {FormsPaths} from "../api.conts";
import {ApplicationViewModel} from "../_Models/Applications/ApplicationViewModel";
import {OwnershipRestructureViewModel} from "../_Models/Applications/OwnershipRestructureViewModel";
import {OwnershipRestructureInputModel} from "../_Models/Applications/OwnershipRestructureInputModel";
import {tap} from "rxjs/operators";
import {WarehouseService} from "./warehouse.service";
import {DisputeInputModel} from "../_Models/Applications/DisputeInputModel";
import {DisputeViewModel} from "../_Models/Applications/DisputeViewModel";
import {ResolveDisputeInputModel} from "../_Models/Applications/ResolveDisputeInputModel";

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
               .pipe(tap(application => this.warehouse.UpdateApplication(application)));
  }

  public Cancel (id: string): Observable<HttpResponse<any>>
  {
    return this.http.delete<HttpResponse<any>>(this.BaseUrl + FormsPaths.Cancel + "/" + encodeURI(id)).pipe(tap(res =>
    {
      console.log("CANCEL APPLICATION before", this.warehouse.MyApplications);
      this.warehouse.RemoveApplication(id);
    }));
  }

  public SubmitCopyrightRegistration (id: string): Observable<CopyrightRegistrationViewModel>
  {
    return this.http.post<CopyrightRegistrationViewModel>(this.BaseUrl + FormsPaths.CopyrightRegistrationSubmit + "/" + id, null)
               .pipe(tap(application => this.warehouse.UpdateApplication(application)));
  }

  public GetMyApplications (): Observable<ApplicationViewModel[]>
  {
    return this.http.get<ApplicationViewModel[]>(this.BaseUrl + FormsPaths.GetMy + "/" + this.authService.UserAccount.getValue().Id)
               .pipe(tap(applications => this.warehouse.MyApplications = applications));
  }

  public GetApplication (id: string): Observable<ApplicationViewModel>
  {
    return this.http.get<ApplicationViewModel>(this.BaseUrl + FormsPaths.BasePath + "/" + id)
               .pipe(tap(application => this.warehouse.UpdateApplication(application)));
  }

  public UpdateOwnershipRestructure (inputModel: OwnershipRestructureInputModel): Observable<OwnershipRestructureViewModel>
  {
    return this.http.post<OwnershipRestructureViewModel>(this.BaseUrl + FormsPaths.OwnershipRestructure, inputModel)
               .pipe(tap(application => this.warehouse.UpdateApplication(application)));
  }

  public SubmitOwnershipRestructure (id: string): Observable<OwnershipRestructureViewModel>
  {
    return this.http.post<OwnershipRestructureViewModel>(this.BaseUrl + FormsPaths.OwnershipRestructureSubmit + "/" + id, null)
               .pipe(tap(application => this.warehouse.UpdateApplication(application)));
  }

  public UpdateDispute (inputModel: DisputeInputModel): Observable<DisputeViewModel>
  {
    return this.http.post<DisputeViewModel>(this.BaseUrl + FormsPaths.Dispute, inputModel)
               .pipe(tap(application => this.warehouse.UpdateApplication(application)));
  }

  public SubmitDispute (id: string): Observable<DisputeViewModel>
  {
    return this.http.post<DisputeViewModel>(this.BaseUrl + FormsPaths.DisputeSubmit + "/" + id, null)
               .pipe(tap(application => this.warehouse.UpdateApplication(application)));
  }

  public ResolveDispute (inputModel: ResolveDisputeInputModel)
  {
    return this.http.post<DisputeViewModel>(this.BaseUrl + FormsPaths.ResolveDispute, inputModel)
               .pipe(tap(application => this.warehouse.UpdateDispute(application)));
  }

  public RecordPayment (disputeId: string, transaction: string)
  {
    return this.http.post(this.BaseUrl + FormsPaths.RecordPayment + "/" + disputeId, null, {params: new HttpParams().set('transaction', transaction)});
  }

  public DeleteUser (): Observable<any>
  {
    return this.http.delete(this.BaseUrl + FormsPaths.DeleteUser + "/" + this.authService.UserAccount.getValue().Id);
  }
}
