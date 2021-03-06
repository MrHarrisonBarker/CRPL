import {Component, Input, OnInit} from '@angular/core';
import {FormArray, FormGroup} from "@angular/forms";

@Component({
  selector: 'ownership-stake-form',
  templateUrl: './ownership-stake-form.component.html',
  styleUrls: ['./ownership-stake-form.component.css'],
})
export class OwnershipStakeFormComponent implements OnInit
{
  @Input() public Index!: number;
  @Input() public Form!: FormGroup;
  @Input() public isDefault: boolean = false;

  ngOnInit (): void
  {
    // console.log("loaded stake!", this.Form)
  }

  constructor ()
  {
  }

  get IsValid ()
  {
    return false;
  }

  public Remove (): void
  {
    (this.Form.parent as FormArray).removeAt(this.Index);
  }

  public Lock (): void
  {
    this.Form.disable();
    console.log(this.Form);
  }

  public UnLock (): void
  {
    this.Form.enable();
    console.log(this.Form);
  }

  get MaximumShares ()
  {
    return ((this.Form.parent as FormArray).parent as FormGroup).value.TotalShares;
  }

  InvalidAndUntouched (control: string) : boolean
  {
    return true;
  }
}
