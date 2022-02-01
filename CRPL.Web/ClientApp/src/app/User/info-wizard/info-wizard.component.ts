import {Component, Input, OnInit} from '@angular/core';
import * as ISO3166 from 'iso-3166-1';
import * as Dial from 'country-telephone-data';
import {Country} from "iso-3166-1/dist/iso-3166";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../../_Services/user.service";
import {AccountInputModel} from "../../_Models/Account/AccountInputModel";
import {AuthService} from "../../_Services/auth.service";
import {Router} from "@angular/router";
import {catchError} from "rxjs/operators";
import {ValidatorsService} from "../../_Services/validators.service";
import {AlertService} from "../../_Services/alert.service";

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

  public FirstPageModel: FormGroup = new FormGroup({
    FirstName: new FormControl('', [Validators.required]),
    LastName: new FormControl('', [Validators.required]),
    RegisteredJurisdiction: new FormControl('', [Validators.required]),
    DOB: new FormControl('', [Validators.required])
  });

  public SecondPageModel: FormGroup = new FormGroup({
    Email: new FormControl('', [this.validators.hasOneContactInfo(), Validators.email], [this.validators.emailValidate()]),
    DialCode: new FormControl('44'),
    PhoneNumber: new FormControl('', [Validators.pattern("^((\\\\+91-?)|0)?[0-9]{10}$")], [this.validators.phoneValidate()])
  });

  public FirstPageErrMessage: string = "";

  constructor (
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private validators: ValidatorsService,
    private alertService: AlertService
  )
  {
  }

  ngOnInit (): void
  {
    if (!this.authService.IsAuthenticated.getValue()) throw new Error("The user is not authenticated!");

    this.PopulateForms();
  }

  public SaveForms (): void
  {
    this.alertService.StartLoading();
    let accountInput: AccountInputModel = {};

    accountInput.FirstName = this.FirstPageModel.value.FirstName;
    accountInput.LastName = this.FirstPageModel.value.LastName;
    accountInput.RegisteredJurisdiction = this.FirstPageModel.value.RegisteredJurisdiction;

    let dob = new Date(this.FirstPageModel.value.DOB)

    console.log(dob);

    accountInput.DateOfBirth = {
      Year: dob.getFullYear(),
      Month: dob.getMonth() + 1,
      Day: dob.getDate()
    }

    accountInput.Email = this.SecondPageModel.value.Email;
    accountInput.DialCode = this.SecondPageModel.value.DialCode;
    accountInput.PhoneNumber = this.SecondPageModel.value.PhoneNumber;

    this.userService.UpdateAccount(accountInput).subscribe(res =>
    {
      if (res)
      {
        this.alertService.Alert({Message: "Saved changes", Type: "success"});
      }
    }, error =>
    {
      console.log("error while updating", error);
      this.alertService.Alert({
        Message: "There was an error updating your information",
        Type: "danger"
      });
    }, () => this.alertService.StopLoading());
  }

  public PopulateForms (): void
  {
    let user = this.authService.UserAccount.getValue();

    this.FirstPageModel.setValue({
      FirstName: user.FirstName,
      LastName: user.LastName,
      RegisteredJurisdiction: user.RegisteredJurisdiction,
      DOB: user.DateOfBirth != null ? `${user.DateOfBirth.Month}/${user.DateOfBirth.Day}/${user.DateOfBirth.Year}` : null
    });

    this.SecondPageModel.setValue({
      Email: user.Email,
      DialCode: user.DialCode,
      PhoneNumber: user.PhoneNumber
    });
  }

  public Closed (): void
  {
    this.open = false;
    if (this.hasChanged()) this.SaveForms();
  }

  public Finished (): void
  {
    this.SaveForms();
  }

  public WizardFinished (): void
  {
    console.log("the wizard has been finished", this.FirstPageModel, this.SecondPageModel);
  }

  private hasChanged (): boolean
  {
    let user = this.authService.UserAccount.getValue();

    if (this.FirstPageModel.value.FirstName != user.FirstName) return true;
    if (this.FirstPageModel.value.LastName != user.LastName) return true;
    if (this.FirstPageModel.value.RegisteredJurisdiction != user.RegisteredJurisdiction) return true;

    let dob = new Date(this.FirstPageModel.value.DOB)

    let dateOfBirth = {
      Year: dob.getFullYear(),
      Month: dob.getMonth() + 1,
      Day: dob.getDate()
    }

    if (this.FirstPageModel.value.DateOfBirth != dateOfBirth) return true;

    if (this.SecondPageModel.value.Email != user.Email) return true;
    if (this.SecondPageModel.value.DialCode != user.DialCode) return true;
    if (this.SecondPageModel.value.PhoneNumber != user.PhoneNumber) return true;

    return false;
  }

  public PageChange (): void
  {
    if (this.hasChanged()) this.SaveForms();
  }
}
