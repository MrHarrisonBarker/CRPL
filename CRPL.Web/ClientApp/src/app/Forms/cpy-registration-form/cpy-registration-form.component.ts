import {Component, ElementRef, Input, OnDestroy, OnInit} from '@angular/core';
import {FormArray, FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../../_Services/auth.service";
import {OwnershipStake} from "../../_Models/StructuredOwnership/OwnershipStake";
import {FormsService} from "../../_Services/forms.service";
import {WorkType} from "../../_Models/WorkType";
import {CopyrightRegistrationInputModel} from "../../_Models/Applications/CopyrightRegistrationInputModel";
import {ValidatorsService} from "../../_Services/validators.service";
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {debounceTime, distinctUntilChanged, map, switchMap, takeUntil, tap} from "rxjs/operators";
import {Observable, of, Subject} from "rxjs";
import {AlertService} from "../../_Services/alert.service";
import {Router} from '@angular/router';

interface ProtectionsMeta
{
  Name: string;
  ReadableName: string;
  Description: string;
}

@Component({
  selector: 'cpy-registration-form',
  templateUrl: './cpy-registration-form.component.html',
  styleUrls: ['./cpy-registration-form.component.css']
})
export class CpyRegistrationFormComponent implements OnInit, OnDestroy
{
  private unsubscribe = new Subject<void>();

  @Input() ExistingApplication!: ApplicationViewModel | CopyrightRegistrationViewModel;

  public RegistrationForm: FormGroup;

  public Protections: ProtectionsMeta[] = [
    {Name: "Authorship",ReadableName: "Authorship", Description: "The eternal right to original authorship."},
    {Name: "CommercialAdaptation",ReadableName: "Commercial adaptation", Description: ""},
    {Name: "NonCommercialAdaptation",ReadableName: "Non-Commercial adaptation", Description: ""},
    {Name: "ReviewOrCrit",ReadableName: "Review or critique", Description: ""},
    {Name: "CommercialPerformance",ReadableName: "Commercial performance", Description: ""},
    {Name: "NonCommercialPerformance",ReadableName: "Non-Commercial performance", Description: ""},
    {Name: "CommercialReproduction",ReadableName: "Commercial reproduction", Description: ""},
    {Name: "NonCommercialReproduction",ReadableName: "Non-Commercial reproduction", Description: ""},
    {Name: "CommercialDistribution",ReadableName: "Commercial distribution", Description: ""},
    {Name: "NonCommercialDistribution",ReadableName: "Non-Commercial distribution", Description: ""}
  ];

  public StandardPreset: string[] = [
    "Authorship", "CommercialAdaptation", "NonCommercialAdaptation", "ReviewOrCrit",
    "CommercialPerformance","NonCommercialPerformance","CommercialReproduction", "NonCommercialReproduction", "CommercialDistribution", "NonCommercialDistribution"];
  public WorkTypes: string[] = Object.values(WorkType).filter(value => typeof value != 'number') as string[];

  constructor (
    private formBuilder: FormBuilder,
    public authService: AuthService,
    private formsService: FormsService,
    private validatorService: ValidatorsService,
    private alertService: AlertService,
    private router: Router,
    private host: ElementRef<HTMLElement>)
  {
    this.RegistrationForm = formBuilder.group({
      Title: ['', [Validators.required]],
      WorkHash: ['', [Validators.required]],
      WorkUri: ['', [Validators.pattern('(https?://)?([\\da-z.-]+)\\.([a-z.]{2,6})[/\\w .-]*/?')]],
      Legal: [''],
      Expires: [100, [Validators.required, Validators.max(100), Validators.min(1)]],
      Protections: formBuilder.group({
        Authorship: [false],
        CommercialAdaptation: [false],
        NonCommercialAdaptation: [false],
        ReviewOrCrit: [false],
        CommercialPerformance: [false],
        NonCommercialPerformance: [false],
        CommercialReproduction: [false],
        NonCommercialReproduction: [false],
        CommercialDistribution: [false],
        NonCommercialDistribution: [false]
      }),
      WorkType: ['Image', [Validators.required]],
      OwnershipStructure: this.formBuilder.group({
        TotalShares: [100, [Validators.required]],
        Stakes: this.formBuilder.array([this.formBuilder.group({
          Owner: ['', [Validators.required], [this.validatorService.RealShareholderValidator()]],
          Share: [1, [Validators.required, Validators.min(1)]]
        })], [this.validatorService.ShareStructureValidator()])
      }),
      AcceptedUla: [false, [Validators.required]]
    });
  }

  get OwnershipStructure (): FormGroup
  {
    return this.RegistrationForm.controls.OwnershipStructure as FormGroup;
  }

  get WorkHash (): FormControl
  {
    return this.RegistrationForm.controls.WorkHash as FormControl;
  }

  public ngOnInit (): void
  {
    // IF NO APPLICATION THEN USE DEFAULT
    if (this.ExistingApplication)
    {
      console.log("application is existing", this.ExistingApplication);
      this.populateForm();
    } else
    {
      (this.OwnershipStructure.controls.Stakes as FormArray).patchValue([{
        Owner: this.authService.UserAccount.getValue().WalletPublicAddress,
        Share: 100
      }])
    }

    this.selectRights(this.StandardPreset);

    this.detectChanges();
  }

  private detectChanges (): void
  {
    this.RegistrationForm.markAsPristine();
    this.RegistrationForm.valueChanges.pipe(distinctUntilChanged()).pipe(
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

  private selectRights (protections: string[]): void
  {
    console.log("selecting rights", protections);
    this.RegistrationForm.controls['Protections'].reset();
    this.Protections.forEach(x => (this.RegistrationForm.controls['Protections'] as FormGroup).controls[x.Name].setValue(false));
    for (let protection in protections)
    {
      let rightsGroup = this.RegistrationForm.controls['Protections'] as FormGroup;
      rightsGroup.controls[this.Protections[protection].Name].setValue(true);
    }
  }

  public ChangeCopyrightType (value: string): void
  {
    switch (value)
    {
      case "Standard":
        this.selectRights(this.StandardPreset);
        break;
      case "Nothing":
        this.selectRights([]);
        break;
    }
  }

  public ChangeWorkType (): void
  {

  }

  private save (): Observable<CopyrightRegistrationViewModel>
  {
    console.log("saving");
    this.alertService.StartLoading();
    let ownership: OwnershipStake[] = this.OwnershipStructure.controls.Stakes.value;

    let inputModel: CopyrightRegistrationInputModel = {
      Title: this.RegistrationForm.value.Title,
      WorkUri: this.RegistrationForm.value.WorkUri,
      Id: this.ExistingApplication != undefined ? this.ExistingApplication.Id : undefined,
      OwnershipStakes: ownership,
      WorkType: this.RegistrationForm.value.WorkType,
      YearsExpire: this.RegistrationForm.value.Expires,
      CopyrightType: this.RegistrationForm.value.ProtectionType,
      Legal: this.RegistrationForm.value.Legal,
      WorkHash: this.RegistrationForm.value.WorkHash,
      Protections: this.RegistrationForm.value.Protections
    }

    return this.formsService.UpdateCopyrightRegistration(inputModel).pipe(tap(crp => {
      this.ExistingApplication = crp;
      console.log("updated reg application", crp, this.ExistingApplication);
    }));
  }

  private populateForm (): void
  {
    let model = this.ExistingApplication = this.ExistingApplication as CopyrightRegistrationViewModel;
    this.RegistrationForm.patchValue({
      Title: model.Title,
      WorkHash: model.WorkHash,
      WorkUri: model.WorkUri,
      Legal: model.Legal,
      Expires: model.YearsExpire,
      ProtectionType: model.CopyrightType,
      WorkType: model.WorkType,
      Protections: model.Protections
    });

    this.OwnershipStructure.patchValue({
      TotalShares: model.OwnershipStakes.map(x => x.Share).reduce((previousValue, currentValue) => previousValue + currentValue)
    });

    (this.OwnershipStructure.controls.Stakes as FormArray).clear();
    for (let stake of model.OwnershipStakes)
    {
      (this.OwnershipStructure.controls.Stakes as FormArray).push(this.generateStake(stake))
    }

    console.log('populated form', this.RegistrationForm.value);
  }

  private generateStake (stake: OwnershipStake)
  {
    return this.formBuilder.group({
      Owner: [stake.Owner, [Validators.required], [this.validatorService.RealShareholderValidator()]],
      Share: [stake.Share, [Validators.required, Validators.min(1)]]
    })
  }

  public Submit (): void
  {
    this.alertService.StartLoading();
    this.unsubscribe.next();
    this.save().subscribe(x =>
    {
        this.formsService.SubmitCopyrightRegistration(this.ExistingApplication.Id).subscribe(x =>
        {
          this.alertService.StopLoading();
          if (this.router.url.includes('/dashboard')) this.host.nativeElement.remove();
          else this.router.navigate(['/dashboard', {applicationId: this.ExistingApplication.Id}]);
        }, error =>
        {
          this.alertService.StopLoading();
          this.alertService.Alert({Type: 'danger', Message: error.error});
        });
    }, error => {
      this.alertService.StopLoading();
      this.alertService.Alert({Type: 'danger', Message: error.error});
    })
  }
}
