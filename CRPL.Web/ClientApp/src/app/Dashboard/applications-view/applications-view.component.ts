import {Component, Input, OnInit} from '@angular/core';
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";

@Component({
  selector: 'applications-view [Application] [ShowForms]',
  templateUrl: './applications-view.component.html',
  styleUrls: ['./applications-view.component.css']
})
export class ApplicationsViewComponent implements OnInit
{
  @Input() Application!: ApplicationViewModel | RegisteredWorkViewModel;
  @Input() ShowForms: boolean = false;

  constructor ()
  {
  }

  ngOnInit (): void
  {
    console.log("Application", this.Application);
  }

  public PropertyInSelected (prop: string): boolean
  {
    if (this.Application) return prop in this.Application;
    return false;
  }

  get ApplicationAsCopyrightRegistration ()
  {
    return (this.Application as CopyrightRegistrationViewModel);
  }

  get ApplicationAsOwnershipRestructure()
  {
    return (this.Application as OwnershipRestructureViewModel);
  }

}
