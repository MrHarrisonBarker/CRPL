import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  NG_VALIDATORS, NG_VALUE_ACCESSOR, ValidationErrors
} from "@angular/forms";
import {OwnershipStake, OwnershipStakeInput} from "../../_Models/StructuredOwnership/OwnershipStake";

@Component({
  selector: 'ownership-stake-input [isDefault]',
  templateUrl: './ownership-stake.component.html',
  styleUrls: ['./ownership-stake.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: OwnershipStakeComponent
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: OwnershipStakeComponent
    },
  ]
})
export class OwnershipStakeComponent implements OnInit, ControlValueAccessor
{
  @Input() isDefault: boolean = false;
  @Input() MaximumShares: number = 100;
  @Input() Disabled = false;

  @Output() DestroyEmit: EventEmitter<any> = new EventEmitter<any>();

  ngOnInit (): void
  {
  }

  constructor ()
  {
  }

  onChange: any = () =>
  {
  }

  onTouch: any = () =>
  {
  }

  public OwnershipStake: OwnershipStakeInput = {
    Owner: "",
    Share: 100,
    Locked: false
  };

  set value (val: OwnershipStakeInput)
  {
    // this value is updated by programmatic changes if( val !== undefined && this.val !== val){
    if (val != null) this.OwnershipStake = val
    this.onChange(val)
    this.onTouch(val)
  }

  get Owner ()
  {
    return this.OwnershipStake.Owner;
  }

  set Owner (val: string)
  {
    this.OwnershipStake.Owner = val;
    this.onChange(this.OwnershipStake)
    this.onTouch(this.OwnershipStake)
  }

  get Share ()
  {
    return this.OwnershipStake.Share;
  }

  set Share (val: number)
  {
    this.OwnershipStake.Share = val;
    this.onChange(this.OwnershipStake)
    this.onTouch(this.OwnershipStake)
  }

  writeValue (value: any)
  {
    this.value = value
  }

  registerOnChange (fn: any)
  {
    this.onChange = fn
  }

  registerOnTouched (fn: any)
  {
    this.onTouch = fn
  }

  validate (control: AbstractControl): ValidationErrors | null
  {
    return null;
  }

  public Remove (): void
  {
    this.DestroyEmit.emit();
  }

  public Lock (): void
  {
    this.OwnershipStake.Locked = true;
    this.onChange(this.OwnershipStake)
    this.onTouch(this.OwnershipStake)
  }

  public UnLock (): void
  {
    this.OwnershipStake.Locked = false;
  }
}
