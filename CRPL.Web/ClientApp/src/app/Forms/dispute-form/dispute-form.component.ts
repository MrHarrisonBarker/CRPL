import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../../_Services/auth.service";
import {FormsService} from "../../_Services/forms.service";
import {ValidatorsService} from "../../_Services/validators.service";
import {AlertService} from "../../_Services/alert.service";
import {Router} from "@angular/router";
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {DisputeViewModel} from "../../_Models/Applications/DisputeViewModel";
import {debounceTime, distinctUntilChanged, switchMap, takeUntil, tap} from "rxjs/operators";
import {Observable, Subject} from "rxjs";
import {DisputeInputModel, DisputeType} from "../../_Models/Applications/DisputeInputModel";

@Component({
  selector: 'dispute-form',
  templateUrl: './dispute-form.component.html',
  styleUrls: ['./dispute-form.component.css']
})
export class DisputeFormComponent implements OnInit, OnDestroy
{
  @Input() RegisteredWork!: RegisteredWorkViewModel;
  @Input() ExistingApplication!: DisputeViewModel;

  public DisputeForm!: FormGroup;
  private unsubscribe = new Subject<void>();

  public DisputeTypes: string[] = Object.values(DisputeType).filter(value => typeof value != 'number') as string[];

  constructor (
    private formBuilder: FormBuilder,
    public authService: AuthService,
    private formsService: FormsService,
    private validatorService: ValidatorsService,
    private alertService: AlertService,
    private router: Router)
  {
    this.DisputeForm = this.formBuilder.group({
      DisputeType: [this.DisputeTypes[0], Validators.required],
      Reason: ['', Validators.required],
      Spotted: ['' , Validators.required],
      Infractions: [1, [Validators.required, Validators.min(1)]],
      ExpectedRecourse: ['', Validators.required],
      ContactAddress: ['', Validators.required],
      LinkToInfraction: ['', Validators.required]
    });
  }

  async ngOnInit (): Promise<any>
  {

    if (!this.authService.IsAuthenticated.getValue()) throw new Error("Not authenticated");

    // NO WORK
    if (!this.RegisteredWork)
    {
      this.alertService.Alert({Type: 'danger', Message: 'No work found!'})
      // this.NoWork = true;
      throw new Error("No work found!");
    }

    // NO APPLICATION
    if (!this.ExistingApplication)
    {
      console.log("NO APPLICATION DO DEFAULT");
      this.detectChanges();
      return;
    }

    // HAS APPLICATION
    if (this.ExistingApplication)
    {
      console.log("HAS APPLICATION SO POP EXISTING");

      return this.formsService.GetApplication(this.ExistingApplication.Id).subscribe(x => {
        this.ExistingApplication = x as DisputeViewModel;
        this.populate();
        this.detectChanges();
      })
    }
  }

  private detectChanges (): void
  {
    this.DisputeForm.markAsPristine();
    this.DisputeForm.valueChanges.pipe(distinctUntilChanged()).pipe(
      debounceTime(1500),
      switchMap(formValue =>
      {
        console.log(formValue);
        return this.save();
      }),
      takeUntil(this.unsubscribe)
    ).subscribe(res =>
    {
      if (res)
      {
        this.alertService.Alert({Message: "Saved changes", Type: "success"});
      }
      ;
    }, error => console.error(error))
  }

  public ngOnDestroy (): void
  {
    this.unsubscribe.next()
  }

  private save(): Observable<DisputeViewModel>
  {
    let inputModel: DisputeInputModel = {
      AccuserId: this.authService.UserAccount.getValue().Id,
      ContactAddress: this.DisputeForm.value.ContactAddress,
      DisputeType: this.DisputeForm.value.DisputeType,
      DisputedWorkId: this.RegisteredWork.Id,
      ExpectedRecourse: this.DisputeForm.value.ExpectedRecourse,
      Id: this.ExistingApplication != undefined ? this.ExistingApplication.Id : undefined,
      Infractions: this.DisputeForm.value.Infractions,
      LinkToInfraction: this.DisputeForm.value.LinkToInfraction,
      Reason: this.DisputeForm.value.Reason,
      Spotted: this.DisputeForm.value.Spotted
    };

    return this.formsService.UpdateDispute(inputModel).pipe(tap(crp =>
    {
      console.log("updated dispute", this.ExistingApplication, crp)
      this.ExistingApplication = crp;
    }));
  }

  private populate (): void
  {

  }
}
