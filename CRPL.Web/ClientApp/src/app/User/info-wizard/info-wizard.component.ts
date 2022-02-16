import {Component, Input, OnInit} from '@angular/core';
import * as ISO3166 from 'iso-3166-1';
import * as Dial from 'country-telephone-data';
import {Country} from "iso-3166-1/dist/iso-3166";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../../_Services/user.service";
import {AccountInputModel} from "../../_Models/Account/AccountInputModel";
import {AuthService} from "../../_Services/auth.service";
import {Router} from "@angular/router";
import {debounceTime, distinctUntilChanged, switchMap, takeUntil} from "rxjs/operators";
import {ValidatorsService} from "../../_Services/validators.service";
import {AlertService} from "../../_Services/alert.service";
import {Observable, Subject} from "rxjs";
import {UserAccountStatusModel} from "../../_Models/Account/UserAccountStatusModel";

@Component({
  selector: 'info-wizard',
  templateUrl: './info-wizard.component.html',
  styleUrls: ['./info-wizard.component.css']
})
export class InfoWizardComponent implements OnInit
{
  @Input() open: boolean = true;

  public countries: Country[] = ISO3166.all();
  public dialCode: Dial.Country[] = Dial.allCountries;

  public UserInfoForm!: FormGroup;

  public FirstPageErrMessage: string = "";
  private unsubscribe = new Subject<void>();

  constructor (
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private validators: ValidatorsService,
    private alertService: AlertService,
    private formBuilder: FormBuilder
  )
  {
    this.UserInfoForm = this.formBuilder.group({
      FirstPage: this.formBuilder.group({
        FirstName: ['', [Validators.required]],
        LastName: ['', [Validators.required]],
        RegisteredJurisdiction: ['', [Validators.required]],
        DOB: ['', [Validators.required]]
      }),
      SecondPage: this.formBuilder.group({
        Email: ['', [this.validators.hasOneContactInfo(), Validators.email], [this.validators.emailValidate()]],
        DialCode: ['44'],
        PhoneNumber: ['', [Validators.pattern("^((\\\\+91-?)|0)?[0-9]{10}$")], [this.validators.phoneValidate()]]
      })
    });
  }

  get FirstPageForm(): FormGroup
  {
    return this.UserInfoForm.controls.FirstPage as FormGroup;
  }

  get SecondPageForm(): FormGroup
  {
    return this.UserInfoForm.controls.SecondPage as FormGroup;
  }

  public ngOnInit (): void
  {
    if (!this.authService.IsAuthenticated.getValue()) throw new Error("The user is not authenticated!");

    this.PopulateForms();
    this.detectChanges();
  }

  public ngOnDestroy (): void
  {
    this.unsubscribe.next()
  }

  private detectChanges (): void
  {
    this.UserInfoForm.markAsPristine();
    this.UserInfoForm.valueChanges.pipe(distinctUntilChanged()).pipe(
      debounceTime(1500),
      switchMap(formValue => this.save()),
      takeUntil(this.unsubscribe)
    ).subscribe(res =>
    {
      if (res)
      {
        this.alertService.Alert({Message: "Saved changes", Type: "success"});
      }

    }, error => console.error(error))
  }

  private save (): Observable<UserAccountStatusModel>
  {

    let accountInput: AccountInputModel = {
      FirstName: this.FirstPageForm.value.FirstName,
      LastName: this.FirstPageForm.value.LastName,
      RegisteredJurisdiction: this.FirstPageForm.value.RegisteredJurisdiction,
      Email: this.SecondPageForm.value.Email,
      DialCode: this.SecondPageForm.value.DialCode,
      PhoneNumber: this.SecondPageForm.value.PhoneNumber
    };

    let dob = new Date(this.FirstPageForm.value.DOB)

    accountInput.DateOfBirth = {
      Year: dob.getFullYear(),
      Month: dob.getMonth() + 1,
      Day: dob.getDate()
    }

    return this.userService.UpdateAccount(accountInput);
  }

  public PopulateForms (): void
  {
    let user = this.authService.UserAccount.getValue();

    this.FirstPageForm.setValue({
      FirstName: user.FirstName,
      LastName: user.LastName,
      RegisteredJurisdiction: user.RegisteredJurisdiction,
      DOB: user.DateOfBirth != null ? `${user.DateOfBirth.Month}/${user.DateOfBirth.Day}/${user.DateOfBirth.Year}` : null
    });

    this.SecondPageForm.setValue({
      Email: user.Email,
      DialCode: user.DialCode,
      PhoneNumber: user.PhoneNumber
    });
  }

  public Closed (): void
  {
    this.open = false;
  }

  public Finished (): void
  {
    this.unsubscribe.next();
    this.save().subscribe();
    this.router.navigate(['/dashboard']);
  }

  public WizardFinished (): void
  {
    console.log("the wizard has been finished", this.UserInfoForm.value);
  }
}
