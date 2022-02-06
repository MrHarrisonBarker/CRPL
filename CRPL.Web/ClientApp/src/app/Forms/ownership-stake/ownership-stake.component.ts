import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor, FormArray, FormGroup,
  NG_VALIDATORS, NG_VALUE_ACCESSOR, ValidationErrors
} from "@angular/forms";
import {OwnershipStake, OwnershipStakeInput} from "../../_Models/StructuredOwnership/OwnershipStake";
import {UserService} from "../../_Services/user.service";
import {UserAccountMinimalViewModel} from "../../_Models/Account/UserAccountMinimalViewModel";

@Component({
  selector: 'ownership-stake-input',
  templateUrl: './ownership-stake.component.html',
  styleUrls: ['./ownership-stake.component.css'],
})
export class OwnershipStakeComponent implements OnInit
{
  @Input() Index!: number;
  @Input() public Form!: FormGroup;

  @Input() isDefault: boolean = false;
  @Input() MaximumShares: number = 100;

  // @Output() DestroyEmit: EventEmitter<any> = new EventEmitter<any>();

  ngOnInit (): void
  {
    console.log("loaded stake!", this.Form)
  }

  constructor (private userService: UserService)
  {
  }

  get IsValid()
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
}
