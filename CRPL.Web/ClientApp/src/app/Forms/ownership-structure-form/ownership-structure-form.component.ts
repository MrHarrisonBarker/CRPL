import {Component, Input, OnInit} from '@angular/core';
import {AuthService} from "../../_Services/auth.service";
import {
  FormArray,
  FormBuilder,
  FormGroup,
  Validators
} from "@angular/forms";
import {ValidatorsService} from "../../_Services/validators.service";

@Component({
  selector: 'ownership-structure-form',
  templateUrl: './ownership-structure-form.component.html',
  styleUrls: ['./ownership-structure-form.component.css']
})
export class OwnershipStructureFormComponent implements OnInit
{
  @Input() public Form!: FormGroup;

  constructor (private authService: AuthService, private fb: FormBuilder, private validatorService: ValidatorsService)
  {
    // console.log("loaded structure!", this.Form)
  }

  private constructStakeGroup (owner: string = ''): FormGroup
  {
    return this.fb.group({
      Owner: [owner, [Validators.required], [this.validatorService.RealShareholderValidator()]],
      Share: [1, [Validators.required, Validators.min(1)]]
    });
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

  public trackByIdx (index: number, obj: any): any
  {
    return index;
  }
}
