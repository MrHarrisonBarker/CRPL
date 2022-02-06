import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../_Services/auth.service";
import {
  AbstractControl, FormArray,
  FormBuilder,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators
} from "@angular/forms";
import {ValidatorsService} from "../../_Services/validators.service";

function ShareStructureValidator() : ValidatorFn
{
  return (control: AbstractControl): ValidationErrors | null =>
  {
    let totalShares = control.parent?.value.TotalShares;
    let count = 0;
    (control as FormArray).controls.forEach(c => count += c.value.Share);
    return count == totalShares ? null : {'InvalidShareStructure': true};
  }
}

@Component({
  selector: 'ownership-structure-input',
  templateUrl: './ownership-structure.component.html',
  styleUrls: ['./ownership-structure.component.css']
})
export class OwnershipStructureComponent implements OnInit
{
  public Form: FormGroup;

  constructor (private authService: AuthService, private fb: FormBuilder, private validatorService: ValidatorsService)
  {
    this.Form = fb.group({
      TotalShares: [100, [Validators.required]],
      Stakes: this.fb.array([this.constructStakeGroup(this.authService.UserAccount.getValue().WalletPublicAddress), this.constructStakeGroup()], [ShareStructureValidator()])
    });
  }

  private constructStakeGroup (owner: string = ''): FormGroup
  {
    return this.fb.group({
      Owner: [owner, [Validators.required], [this.validatorService.RealShareholderValidator()]],
      Share: [1, [Validators.required]]
    })
  }

  public ngOnInit (): void
  {
  }

  get StakeArray (): FormArray
  {
    return (this.Form.controls.Stakes as FormArray)
  }

  get Stakes (): FormGroup[]
  {
    return this.StakeArray.controls as FormGroup[];
  }

  public AddStake (): void
  {
    this.StakeArray.push(this.constructStakeGroup());
  }
}
