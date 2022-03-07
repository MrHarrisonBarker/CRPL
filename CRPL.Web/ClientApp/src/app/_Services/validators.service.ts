import {Injectable} from '@angular/core';
import {AbstractControl, AsyncValidatorFn, FormArray, ValidationErrors, ValidatorFn} from "@angular/forms";
import {Observable, of} from "rxjs";
import {UserService} from "./user.service";
import {map} from "rxjs/operators";
import {isEmptyInputValue} from "../utils";

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
      console.log(control.value);
      if (isEmptyInputValue(control.value)) return of(null);
      return this.userService.IsPhoneUnique(control.value).pipe(map(res =>
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
      if (isEmptyInputValue(control.value)) return of(null);
      return this.userService.IsEmailUnique(control.value).pipe(map(res =>
      {
        return res ? null : {emailExists: true}
      }));
    }
  }

  public ShareStructureValidator (): ValidatorFn
  {
    return (control: AbstractControl): ValidationErrors | null =>
    {
      let totalShares = control.parent?.value.TotalShares;
      let count = 0;
      (control as FormArray).controls.forEach(c => count += c.value.Share);
      return count == totalShares ? null : {'InvalidShareStructure': true};
    }
  }

  public RealShareholderValidator (): AsyncValidatorFn
  {
    return (control: AbstractControl): Observable<ValidationErrors | null> =>
    {
      return this.userService.IsReal(control.value).pipe(map(x => x ? null : {'NotRealShareholder': true}));
    }
  }
}
