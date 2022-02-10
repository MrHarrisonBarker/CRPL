import {AfterViewInit, Component, Input, OnDestroy, OnInit} from '@angular/core';
import {FormArray, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../../_Services/auth.service";
import {FormsService} from "../../_Services/forms.service";
import {ValidatorsService} from "../../_Services/validators.service";
import {AlertService} from "../../_Services/alert.service";
import {Router} from "@angular/router";
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {OwnershipRestructureInputModel} from "../../_Models/Applications/OwnershipRestructureInputModel";
import {debounceTime, switchMap, takeUntil, tap} from "rxjs/operators";
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";
import {Observable, of, Subject} from "rxjs";
import {OwnershipStake} from "../../_Models/StructuredOwnership/OwnershipStake";

@Component({
  selector: 'cpy-restructure-form',
  templateUrl: './cpy-restructure-form.component.html',
  styleUrls: ['./cpy-restructure-form.component.css']
})
export class CpyRestructureFormComponent implements OnInit, OnDestroy, AfterViewInit
{
  @Input() RegisteredWork!: RegisteredWorkViewModel;
  @Input() ExistingApplication!: OwnershipRestructureViewModel | undefined;

  public RestructureForm!: FormGroup;
  public NoWork: boolean = false;

  private unsubscribe = new Subject<void>();

  constructor (
    private formBuilder: FormBuilder,
    public authService: AuthService,
    private formsService: FormsService,
    private validatorService: ValidatorsService,
    private alertService: AlertService,
    private router: Router)
  {
    this.RestructureForm = this.formBuilder.group({
      CurrentStructure: this.formBuilder.group({
        TotalShares: [100, [Validators.required]],
        Stakes: this.formBuilder.array([this.formBuilder.group({
          Owner: ['', [Validators.required], [this.validatorService.RealShareholderValidator()]],
          Share: [1, [Validators.required, Validators.min(1)]]
        })], [this.validatorService.ShareStructureValidator()])
      }),
      ProposedStructure: this.formBuilder.group({
        TotalShares: [100, [Validators.required]],
        Stakes: this.formBuilder.array([this.formBuilder.group({
          Owner: ['', [Validators.required], [this.validatorService.RealShareholderValidator()]],
          Share: [1, [Validators.required, Validators.min(1)]]
        })], [this.validatorService.ShareStructureValidator()])
      }),
      AcceptedUla: [false, [Validators.required]]
    });
  }

  ngOnInit (): void
  {
    console.log("starting restructure application", this.ExistingApplication, this.RegisteredWork);
    if (!this.RegisteredWork)
    {
      this.alertService.Alert({Type: 'danger', Message: 'No work found!'})
      this.NoWork = true;
    } else this.populateCurrentStructure();

    if (this.ExistingApplication)
    {
      console.log("There is an existing application", this.ExistingApplication);
      this.formsService.GetApplication(this.ExistingApplication.Id).subscribe(x =>
      {
        this.ExistingApplication = x as OwnershipRestructureViewModel;
        this.populate();
      });
    } else (this.ProposedStructure.controls.Stakes as FormArray).patchValue([{
      Owner: this.authService.UserAccount.getValue().WalletPublicAddress,
      Share: 100
    }]);
  }

  ngAfterViewInit (): void
  {
    this.RestructureForm.valueChanges.pipe(
      debounceTime(1500),
      switchMap(formValue =>
      {
        if (!this.RestructureForm.pending) return this.save()
        return of(null);
      }),
      takeUntil(this.unsubscribe)
    ).subscribe(res =>
    {
      if (res)
      {
        this.alertService.Alert({Message: "Saved changes", Type: "success"});
      }
      this.alertService.StopLoading();
    }, error => console.error(error))
  }

  public ngOnDestroy (): void
  {
    this.unsubscribe.next()
  }

  get ProposedStructure (): FormGroup
  {
    return this.RestructureForm.controls.ProposedStructure as FormGroup;
  }

  get CurrentStructure (): FormGroup
  {
    return this.RestructureForm.controls.CurrentStructure as FormGroup;
  }

  private populate ()
  {
    if (this.ExistingApplication)
    {
      (this.ProposedStructure.controls.Stakes as FormArray).clear();
      for (let stake of this.ExistingApplication.ProposedStructure)
      {
        (this.ProposedStructure.controls.Stakes as FormArray).push(this.generateStake(stake))
      }

      this.ProposedStructure.patchValue({
        TotalShares: this.ExistingApplication.ProposedStructure.map(x => x.Share).reduce((previousValue, currentValue) => previousValue + currentValue),
      });

      (this.CurrentStructure.controls.Stakes as FormArray).clear();
      for (let stake of this.ExistingApplication.CurrentStructure)
      {
        (this.CurrentStructure.controls.Stakes as FormArray).push(this.generateStake(stake))
      }

      this.CurrentStructure.patchValue({
        TotalShares: this.ExistingApplication.CurrentStructure.map(x => x.Share).reduce((previousValue, currentValue) => previousValue + currentValue),
      });
      console.log("application populated", this.RestructureForm.value)
    }
  }

  private generateStake (stake: OwnershipStake)
  {
    return this.formBuilder.group({
      Owner: [stake.Owner, [Validators.required], [this.validatorService.RealShareholderValidator()]],
      Share: [stake.Share, [Validators.required, Validators.min(1)]]
    })
  }

  private populateCurrentStructure ()
  {
    if (this.RegisteredWork.OwnershipStructure)
    {
      let formArr = (this.CurrentStructure.controls.Stakes as FormArray);
      formArr.clear()
      this.RegisteredWork.OwnershipStructure.forEach(s => formArr.push(this.generateStake(s)))
      this.CurrentStructure.patchValue({TotalShares: this.RegisteredWork.OwnershipStructure.map(x => x.Share).reduce((previousValue, currentValue) => previousValue + currentValue),});

      console.log("populated current struct", this.RestructureForm.value);
    }
  }

  private save (): Observable<OwnershipRestructureViewModel>
  {
    this.alertService.StartLoading();
    console.log("saving restructure application");

    let inputModel: OwnershipRestructureInputModel = {
      CurrentStructure: this.CurrentStructure.controls.Stakes.value,
      ProposedStructure: this.ProposedStructure.controls.Stakes.value
    }

    if (this.RegisteredWork) inputModel.WorkId = this.RegisteredWork.Id;
    if (this.ExistingApplication) inputModel.Id = this.ExistingApplication.Id;

    return this.formsService.UpdateOwnershipRestructure(inputModel).pipe(tap(crp => this.ExistingApplication = crp));
  }

  public Submit (): void
  {
    this.alertService.StartLoading();
    // stops change detection and auto save
    this.unsubscribe.next();
    if (this.ExistingApplication?.Id) this.formsService.SubmitOwnershipRestructure(this.ExistingApplication.Id).subscribe();
  }
}
