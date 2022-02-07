import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {Form, FormArray, FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../../_Services/auth.service";
import {OwnershipStake} from "../../_Models/StructuredOwnership/OwnershipStake";
import {FormsService} from "../../_Services/forms.service";
import {WorkType} from "../../_Models/WorkType";
import {CopyrightRegistrationInputModel} from "../../_Models/Applications/CopyrightRegistrationInputModel";
import {ValidatorsService} from "../../_Services/validators.service";
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {debounceTime, switchMap, takeUntil} from "rxjs/operators";
import {Observable, Subject} from "rxjs";
import {AlertService} from "../../_Services/alert.service";

interface RightMeta
{
  Name: string;
  Description: string;
}

@Component({
  selector: 'cpy-registration',
  templateUrl: './cpy-registration.component.html',
  styleUrls: ['./cpy-registration.component.css']
})
export class CpyRegistrationComponent implements OnInit, OnDestroy
{
  @Input() ExistingApplication!: ApplicationViewModel;
  public RegistrationForm: FormGroup;
  public AcceptedUla: boolean = false;

  public Rights: RightMeta[] = [
    {Name: "authorship", Description: "The eternal right to original authorship."},
    {
      Name: "reproduce",
      Description: "The exclusive right to reproduction and the right to authorise any other party for reproduction."
    },
    {
      Name: "distribution",
      Description: "The exclusive right to distribution and the right to authorise any other party for distribution including but not limited to; online distribution markets, public performances or broadcasts."
    },
    {
      Name: "adapt",
      Description: "The exclusive right to adapt and recite work while maintaining all rights to the derivative including the right to seek additional protection on the derivative works."
    },
    {
      Name: "changeOfOwnership",
      Description: "The exclusive right to alter the underlying ownership of the work in- cluding removing the original party as principle owner of the work."
    },
  ];

  public StandardRights: string[] = ["authorship", "reproduce", "distribution", "adapt", "changeOfOwnership"];
  public PermissiveRights: string[] = ["authorship"];
  public CopyleftRights: string[] = ["authorship", "reproduce"];
  public WorkTypes: string[] = Object.values(WorkType).filter(value => typeof value != 'number') as string[];

  constructor (
    private formBuilder: FormBuilder,
    public authService: AuthService,
    private formsService: FormsService,
    private validatorService: ValidatorsService,
    private alertService: AlertService)
  {
    this.RegistrationForm = formBuilder.group({
      Title: ['', [Validators.required]],
      WorkHash: ['', [Validators.required]],
      WorkUri: ['', [Validators.pattern('(https?://)?([\\da-z.-]+)\\.([a-z.]{2,6})[/\\w .-]*/?')]],
      Legal: [''],
      Expires: [100, [Validators.required, Validators.max(100), Validators.min(1)]],
      ProtectionType: ['Standard', [Validators.required]],
      Rights: formBuilder.group({
        authorship: [false],
        reproduce: [false],
        distribution: [false],
        adapt: [false],
        changeOfOwnership: [false]
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

  private unsubscribe = new Subject<void>();

  public ngOnInit (): void
  {
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

    this.selectRights(this.StandardRights);

    this.RegistrationForm.valueChanges.pipe(
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

  private selectRights (rights: string[]): void
  {
    this.RegistrationForm.controls['Rights'].reset();
    for (let right in rights)
    {
      let rightsGroup = this.RegistrationForm.controls['Rights'] as FormGroup;
      rightsGroup.controls[this.StandardRights[right]].setValue(true);
    }
  }

  public ChangeCopyrightType (): void
  {
    switch (this.RegistrationForm.value.ProtectionType)
    {
      case "Standard":
        this.selectRights(this.StandardRights);
        break;
      case "Permissive":
        this.selectRights(this.PermissiveRights);
        break;
      case "Copyleft":
        this.selectRights(this.CopyleftRights);
        break;
    }
  }

  public ChangeWorkType (): void
  {

  }

  public save (): Observable<CopyrightRegistrationViewModel>
  {
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
      WorkHash: this.RegistrationForm.value.WorkHash
    }

    return this.formsService.UpdateCopyrightRegistration(inputModel);
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
    });

    this.OwnershipStructure.patchValue({
      TotalShares: model.OwnershipStakes.map(x => x.Share).reduce((previousValue, currentValue) => previousValue + currentValue),
      Stakes: model.OwnershipStakes
    });

    console.log('populated form', this.RegistrationForm.value);
  }
}
