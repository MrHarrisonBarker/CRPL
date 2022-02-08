import {Component, Input, OnInit} from '@angular/core';
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";

@Component({
  selector: 'applications-view [Application]',
  templateUrl: './applications-view.component.html',
  styleUrls: ['./applications-view.component.css']
})
export class ApplicationsViewComponent implements OnInit
{
  @Input() Application!: ApplicationViewModel | RegisteredWorkViewModel;

  constructor ()
  {
  }

  ngOnInit (): void
  {
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

}
