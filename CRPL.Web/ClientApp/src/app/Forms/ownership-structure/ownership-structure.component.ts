import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../_Services/auth.service";
import {
  Form,
  FormArray, FormBuilder, FormGroup, Validators
} from "@angular/forms";

@Component({
  selector: 'ownership-structure-input',
  templateUrl: './ownership-structure.component.html',
  styleUrls: ['./ownership-structure.component.css']
})
export class OwnershipStructureComponent implements OnInit
{
  public Form: FormGroup;

  constructor (private authService: AuthService, private fb: FormBuilder)
  {
    this.Form = fb.group({
      TotalShares: [100, [Validators.required]],
      Stakes: this.fb.array([this.constructStakeGroup(this.authService.UserAccount.getValue().WalletPublicAddress), this.constructStakeGroup()])
    });
  }

  private constructStakeGroup(owner: string = ''): FormGroup
  {
    return this.fb.group({
      Owner: [owner, [Validators.required]],
      Share: [1, [Validators.required]]
    })
  }

  public ngOnInit (): void
  {
  }

  get StakeArray() : FormArray {
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
