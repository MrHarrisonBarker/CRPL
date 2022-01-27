import {Component, Input, OnInit} from '@angular/core';
// const iso = require('iso-3166-1');
import Index from 'iso-3166-1';
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
  countries: Country[] = Index.all();
  @Input() open: boolean = true;
  formPageOneValid: boolean = true;
  formPageTwoValid: boolean = false;

  public FirstPageModel: FormGroup = new FormGroup({
    FirstName: new FormControl('', [Validators.required]),
    LastName: new FormControl('', [Validators.required]),
    RegisteredJurisdiction: new FormControl('', [Validators.required]),
    DOB: new FormControl('', [Validators.required])
  });

  public SecondPageMode: FormGroup = new FormGroup({
    Email: new FormControl('', [Validators.required]),
    PhoneNumber: new FormControl('', [Validators.required], [this.validators.phoneValidate()])
  });

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

  public SaveForms ()
  {
    let accountInput: AccountInputModel = {};

    accountInput.FirstName = this.FirstPageModel.value.FirstName;
    accountInput.LastName = this.FirstPageModel.value.LastName;
    accountInput.RegisteredJurisdiction = this.FirstPageModel.value.RegisteredJurisdiction;

    let dob = new Date(this.FirstPageModel.value.DOB)

    console.log(dob);

    accountInput.DateOfBirth = {
      Year: dob.getFullYear(),
      Month: dob.getMonth()+1,
      Day: dob.getDate()
    }

    accountInput.Email = this.SecondPageMode.value.Email;
    accountInput.PhoneNumber = this.SecondPageMode.value.PhoneNumber;

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

    this.SecondPageMode.setValue({
      Email: user.Email,
      PhoneNumber: user.PhoneNumber
    });
  }

  public reset ($event: boolean)
  {
    this.open = false;
    console.log($event, this.FirstPageModel.value, this.SecondPageMode.value);
    this.SaveForms();
  }

  public finished ($event: any)
  {
    console.log($event);
  }
}
