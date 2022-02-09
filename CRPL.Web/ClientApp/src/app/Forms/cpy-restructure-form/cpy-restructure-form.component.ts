import {Component, Input, OnDestroy, OnInit} from '@angular/core';
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
import {Observable, Subject} from "rxjs";

@Component({
  selector: 'cpy-restructure-form',
  templateUrl: './cpy-restructure-form.component.html',
  styleUrls: ['./cpy-restructure-form.component.css']
})
export class CpyRestructureFormComponent implements OnInit, OnDestroy
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
    if (!this.RegisteredWork)
    {
      this.alertService.Alert({Type: 'danger', Message: 'No work found!'})
      this.NoWork = true;
    } else this.populateCurrentStructure();

    if (this.ExistingApplication)
    {
      console.log("There is an existing application", this.ExistingApplication);
      this.formsService.GetApplication(this.ExistingApplication.Id).subscribe(x => {
        this.ExistingApplication = x as OwnershipRestructureViewModel;
        this.populate();
      });
    } else (this.ProposedStructure.controls.Stakes as FormArray).patchValue([{
      Owner: this.authService.UserAccount.getValue().WalletPublicAddress,
      Share: 100
    }]);

    this.RestructureForm.valueChanges.pipe(
      debounceTime(1500),
      switchMap(formValue => this.save()),
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
      this.ProposedStructure.patchValue({
        TotalShares: this.ExistingApplication.ProposedStructure.map(x => x.Share).reduce((previousValue, currentValue) => previousValue + currentValue),
        Stakes: this.ExistingApplication.ProposedStructure
      });

      this.CurrentStructure.patchValue({
        TotalShares: this.ExistingApplication.CurrentStructure.map(x => x.Share).reduce((previousValue, currentValue) => previousValue + currentValue),
        Stakes: this.ExistingApplication.CurrentStructure
      });
    }
  }

  private populateCurrentStructure ()
  {
    if (this.RegisteredWork.OwnershipStructure)
    {
      this.CurrentStructure.patchValue({
        TotalShares: this.RegisteredWork.OwnershipStructure.map(x => x.Share).reduce((previousValue, currentValue) => previousValue + currentValue),
        Stakes: this.RegisteredWork.OwnershipStructure
      });
    }
  }

  private save (): Observable<OwnershipRestructureViewModel>
  {
    console.log("saving restructure application");

    let inputModel: OwnershipRestructureInputModel = {
      CurrentStructure: this.CurrentStructure.controls.Stakes.value,
      ProposedStructure: this.ProposedStructure.controls.Stakes.value
    }

    if (this.ExistingApplication) inputModel.WorkId = this.ExistingApplication.Id;

    return this.formsService.UpdateOwnershipRestructure(inputModel).pipe(tap(crp => this.ExistingApplication = crp));
  }
}
