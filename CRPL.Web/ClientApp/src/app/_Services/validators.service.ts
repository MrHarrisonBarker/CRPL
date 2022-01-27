import {Injectable} from '@angular/core';
import {AbstractControl, AsyncValidatorFn, ValidationErrors} from "@angular/forms";
import {Observable} from "rxjs";
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
    console.log("validating if phone exists")
    return (control: AbstractControl): Observable<ValidationErrors | null> =>
    {
      console.log("checking if phone exists")
      return this.userService.PhoneExists(control.value).pipe(map(res => {
        console.log("phone is", res);
        return res ? null : {phoneExists: true}
      }));
    }
  }

}
