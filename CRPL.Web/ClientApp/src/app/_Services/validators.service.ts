import {Injectable} from '@angular/core';
import {AbstractControl, AsyncValidatorFn, FormGroup, ValidationErrors, ValidatorFn} from "@angular/forms";
import {Observable, of} from "rxjs";
import {UserService} from "./user.service";
import {map} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class ValidatorsService
{

  constructor (private userService: UserService)
  {
  }

  public phoneValidate (): AsyncValidatorFn
  {
    return (control: AbstractControl): Observable<ValidationErrors | null> =>
    {
      if (control.value == null || control.value == '') return of(null);
      return this.userService.PhoneExists(control.value).pipe(map(res =>
      {
        console.log("phone is", res);
        return res ? null : {phoneExists: true}
      }));
    }
  }

  public emailValidate (): AsyncValidatorFn
  {
    return (control: AbstractControl): Observable<ValidationErrors | null> =>
    {
      if (control.value == null || control.value == '') return of(null);
      return this.userService.EmailExists(control.value).pipe(map(res =>
      {
        return res ? null : {emailExists: true}
      }));
    }
  }

  public hasOneContactInfo (group: FormGroup): boolean
  {
      return group.value.PhoneNumber != "" || group.value.PhoneNumber != ""
  }

}
