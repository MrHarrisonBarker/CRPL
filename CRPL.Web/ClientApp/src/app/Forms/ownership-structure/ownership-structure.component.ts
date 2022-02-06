import {Component, OnInit} from '@angular/core';
import {OwnershipStakeInput} from "../../_Models/StructuredOwnership/OwnershipStake";
import {AuthService} from "../../_Services/auth.service";
import {
  AbstractControl,
  ControlValueAccessor,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors
} from "@angular/forms";

@Component({
  selector: 'ownership-structure-input',
  templateUrl: './ownership-structure.component.html',
  styleUrls: ['./ownership-structure.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: OwnershipStructureComponent
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: OwnershipStructureComponent
    },
  ]
})
export class OwnershipStructureComponent implements OnInit, ControlValueAccessor
{
  public OwnershipStakes: OwnershipStakeInput[] = [];
  public TotalShares: number = 100;

  public trackByIdx (index: number, obj: any): any
  {
    return index;
  }

  onChange: any = () =>
  {
  }

  onTouch: any = () =>
  {
  }

  constructor (private authService: AuthService)
  {
  }

  public ngOnInit (): void
  {
    this.OwnershipStakes.push({
      Owner: this.authService.UserAccount.getValue().WalletPublicAddress,
      Share: this.TotalShares,
      Locked: false
    });
  }

  public AddStake (): void
  {
    this.OwnershipStakes.push({Owner: "", Share: 1, Locked: false});
    this.onTouch(this.OwnershipStakes);
    this.onChange(this.OwnershipStakes);
  }

  public DestroyStake (i: number): void
  {
    this.OwnershipStakes.splice(i, 1);
    this.onTouch(this.OwnershipStakes);
    this.onChange(this.OwnershipStakes);
  }

  public IsOwnershipValid (): boolean
  {
    let count = 0;
    for (let ownershipStake of this.OwnershipStakes)
    {
      count += ownershipStake.Share;
    }
    return count == this.TotalShares;
  }

  public CalculateMaximumShares (i: number): number
  {
    if (this.OwnershipStakes[i].Locked) return this.OwnershipStakes[i].Share;

    let areSomeLocked = this.OwnershipStakes.find(x => x.Locked);

    if (areSomeLocked)
    {
      let lockedShares = this.OwnershipStakes.filter(x => x.Locked).map(x => x.Share);
      let totalLockedShares = lockedShares.reduce((sum, current) => sum + current, 0);
      return (this.TotalShares - totalLockedShares) - ((this.OwnershipStakes.length - lockedShares.length) - 1);
    }

    return this.TotalShares - (this.OwnershipStakes.length - 1);
  }

  writeValue (value: any)
  {
    // this.OwnershipStakes = value
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

  public StakeChanged (index: number): void
  {
    this.onTouch(this.OwnershipStakes);
    this.onChange(this.OwnershipStakes);
  }
}
