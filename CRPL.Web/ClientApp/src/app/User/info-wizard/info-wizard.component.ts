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

@Component({
  selector: 'info-wizard',
  templateUrl: './info-wizard.component.html',
  styleUrls: ['./info-wizard.component.css']
})
export class InfoWizardComponent implements OnInit
{
  countries: Country[] = ISO3166.all();
  dialCode: Dial.Country[] = Dial.allCountries;
  @Input() open: boolean = true;

  public FirstPageModel: FormGroup = new FormGroup({
    FirstName: new FormControl('', [Validators.required]),
    LastName: new FormControl('', [Validators.required]),
    RegisteredJurisdiction: new FormControl('', [Validators.required]),
    DOB: new FormControl('', [Validators.required])
  });

  public SecondPageModel: FormGroup = new FormGroup({
    Email: new FormControl('', [Validators.email], [this.validators.emailValidate()]),
    DialCode: new FormControl('44'),
    PhoneNumber: new FormControl('', [Validators.pattern("^((\\\\+91-?)|0)?[0-9]{10}$")], [this.validators.phoneValidate()])
  });

  SecondPageErrMessage: string = "";
  FirstPageErrMessage: string = "";

  constructor (
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private validators: ValidatorsService
  )
  {
  }

  ngOnInit (): void
  {
    console.log("[info wizard] init");

    if (!this.authService.IsAuthenticated.getValue()) throw new Error("The user is not authenticated!");

    this.PopulateForms();
  }

  private validateForms ()
  {
    this.SecondPageErrMessage = "";

    let valid = true;

    if(!this.validators.hasOneContactInfo(this.SecondPageModel))
    {
      valid = false;
      this.SecondPageErrMessage += "You must have one way of contacting";
    }

    // if entered a phone number then country
    // if (this.SecondPageModel.value.PhoneNumber != '')
    // {
    //
    // }

    return valid;
  }

  public SaveForms ()
  {
    if (!this.validateForms()) return;

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

    this.userService.UpdateAccount(accountInput).pipe(catchError(err => err)).subscribe();
  }

  public PopulateForms ()
  {
    let user = this.authService.UserAccount.getValue();

    this.FirstPageModel.setValue({
      FirstName: user.FirstName,
      LastName: user.LastName,
      RegisteredJurisdiction: user.RegisteredJurisdiction,
      DOB: `${user.DateOfBirth.Month}/${user.DateOfBirth.Day}/${user.DateOfBirth.Year}`
    });

    this.SecondPageModel.setValue({
      Email: user.Email,
      DialCode: user.DialCode,
      PhoneNumber: user.PhoneNumber
    });
  }

  public reset ($event: boolean)
  {
    this.open = false;
    console.log($event, this.FirstPageModel.value, this.SecondPageModel.value);
    // this.SaveForms();
  }

  public finished ($event: any)
  {
    console.log($event);
  }

  public FinishWizard ()
  {
    console.log("the wizard has been finished", this.FirstPageModel, this.SecondPageModel)
    console.log("valids", this.FirstPageModel.valid, this.SecondPageModel.valid, this.SecondPageModel.controls.PhoneNumber.valid);
  }
}
