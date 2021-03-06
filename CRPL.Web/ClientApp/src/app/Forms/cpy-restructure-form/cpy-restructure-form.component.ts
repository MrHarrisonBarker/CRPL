import {Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges} from '@angular/core';
import {FormArray, FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../../_Services/auth.service";
import {FormsService} from "../../_Services/forms.service";
import {ValidatorsService} from "../../_Services/validators.service";
import {AlertService} from "../../_Services/alert.service";
import {Router} from "@angular/router";
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {OwnershipRestructureInputModel} from "../../_Models/Applications/OwnershipRestructureInputModel";
import {debounceTime, distinctUntilChanged, finalize, switchMap, takeUntil, tap} from "rxjs/operators";
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";
import {Observable, Subject, Subscription} from "rxjs";
import {OwnershipStake} from "../../_Models/StructuredOwnership/OwnershipStake";
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";

@Component({
  selector: 'cpy-restructure-form [RegisteredWork]',
  templateUrl: './cpy-restructure-form.component.html',
  styleUrls: ['./cpy-restructure-form.component.css']
})
export class CpyRestructureFormComponent implements OnInit, OnDestroy, OnChanges
{
  @Output() Open: EventEmitter<boolean> = new EventEmitter<boolean>();

  @Input() ApplicationAsync!: Observable<ApplicationViewModel>;
  private ApplicationSubscription!: Subscription;

  @Input() RegisteredWork!: RegisteredWorkViewModel;
  @Input() ExistingApplication!: OwnershipRestructureViewModel;

  public RestructureForm!: FormGroup;
  public Accepted!: FormControl;
  public NoWork: boolean = false;

  private unsubscribe = new Subject<void>();
  public Locked: boolean = false;

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
      })
    });
    this.Accepted = this.formBuilder.control(false, [Validators.required]);
  }

  private subscribeToApplication() : void
  {
    this.ApplicationSubscription = this.ApplicationAsync.subscribe(application =>
    {
      this.unsubscribe.next();
      this.ExistingApplication = application as OwnershipRestructureViewModel;
      this.populate();
      this.detectChanges();
    });
  }

  async ngOnInit (): Promise<any>
  {
    console.log("starting restructure application", this.ExistingApplication, this.RegisteredWork);

    // NO WORK
    if (!this.RegisteredWork)
    {
      this.alertService.Alert({Type: 'danger', Message: 'No work found!'})
      this.NoWork = true;
      throw new Error("No work found!");
    }

    // NO CURRENT STRUCTURE
    if (!this.RegisteredWork.OwnershipStructure)
    {
      this.NoWork = true;
      throw new Error("No current structure found!");
    }

    // NO APPLICATION
    if (!this.ExistingApplication)
    {
      console.log("NO APPLICATION SO POP DEFAULT AND CURRENT STRUCTURE");
      // POP DEFAULT APPLICATION
      this.populateStakes(this.ProposedStructure, [{
        Owner: this.authService.UserAccount.getValue().WalletPublicAddress,
        Share: 100
      }])
      this.populateStakes(this.CurrentStructure, this.RegisteredWork.OwnershipStructure);
      this.detectChanges();
      return;
    }

    // HAS APPLICATION
    if (this.ExistingApplication)
    {
      console.log("HAS APPLICATION SO POP EXISTING");
      // POP EXISTING APPLICATION
      return this.formsService.GetApplication(this.ExistingApplication.Id).subscribe(x =>
      {
        this.ExistingApplication = x as OwnershipRestructureViewModel;
        this.populate();
        this.detectChanges();
      });
    }
  }

  private detectChanges (): void
  {
    this.RestructureForm.markAsPristine();
    this.RestructureForm.valueChanges.pipe(distinctUntilChanged()).pipe(
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

  get ProposedStructure (): FormGroup
  {
    return this.RestructureForm.controls.ProposedStructure as FormGroup;
  }

  get CurrentStructure (): FormGroup
  {
    return this.RestructureForm.controls.CurrentStructure as FormGroup;
  }

  private populateStakes (group: FormGroup, structure: OwnershipStake[]): void
  {
    let array = group.controls.Stakes as FormArray;
    array.clear();
    structure.forEach(stake => array.push(this.generateStake(stake)));

    group.patchValue({
      TotalShares: structure.map(x => x.Share).reduce((previousValue, currentValue) => previousValue + currentValue)
    }, {emitEvent: false});
  }

  private populate ()
  {
    this.populateStakes(this.ProposedStructure, this.ExistingApplication.ProposedStructure);
    this.populateStakes(this.CurrentStructure, this.ExistingApplication.CurrentStructure);
  }

  private generateStake (stake: OwnershipStake)
  {
    return this.formBuilder.group({
      Owner: [stake.Owner, [Validators.required], [this.validatorService.RealShareholderValidator()]],
      Share: [stake.Share, [Validators.required, Validators.min(1)]]
    })
  }

  private save (): Observable<OwnershipRestructureViewModel>
  {

    console.log("saving restructure application");

    let inputModel: OwnershipRestructureInputModel = {
      CurrentStructure: this.CurrentStructure.controls.Stakes.value,
      ProposedStructure: this.ProposedStructure.controls.Stakes.value
    }

    if (this.RegisteredWork) inputModel.WorkId = this.RegisteredWork.Id;
    if (this.ExistingApplication) inputModel.Id = this.ExistingApplication.Id;

    return this.formsService.UpdateOwnershipRestructure(inputModel).pipe(tap(crp =>
    {
      console.log("updated restructure", this.ExistingApplication, crp)
      this.ExistingApplication = crp;
    }));
  }

  public Submit (): void
  {
    this.Locked = true;
    // stops change detection and auto save
    this.unsubscribe.next();
    this.save()
        .pipe(finalize(() => this.Locked = false))
        .subscribe(x =>
        {
          if (this.ExistingApplication?.Id)
          {
            this.formsService.SubmitOwnershipRestructure(this.ExistingApplication.Id)
                .pipe(finalize(() => this.Locked = false))
                .subscribe(x =>
                  {
                    this.Open.emit(false);
                    this.router.navigate(['/dashboard', {applicationId: this.ExistingApplication.Id}])
                  },
                  error => this.alertService.Alert({
                    Type: 'danger',
                    Message: 'There was an error submitting a ownership restructure!'
                  }));
          }
        }, err => this.alertService.Alert({Type: 'danger', Message: err.error}));
  }

  public ngOnDestroy (): void
  {
    this.unsubscribe.next();
    if (this.ApplicationSubscription) this.ApplicationSubscription.unsubscribe();
  }

  public ngOnChanges (changes: SimpleChanges): void
  {
    if (this.ApplicationSubscription)
    {
      this.ApplicationSubscription.unsubscribe();
      this.subscribeToApplication();
    }
  }
}
