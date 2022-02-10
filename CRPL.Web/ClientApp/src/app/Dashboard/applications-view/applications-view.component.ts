import {Component, Input, OnInit} from '@angular/core';
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";
import {WarehouseService} from "../../_Services/warehouse.service";

@Component({
  selector: 'applications-view [Application] [ShowForms]',
  templateUrl: './applications-view.component.html',
  styleUrls: ['./applications-view.component.css']
})
export class ApplicationsViewComponent implements OnInit
{
  @Input() Application!: ApplicationViewModel;
  @Input() ShowForms: boolean = false;

  constructor (private warehouse: WarehouseService)
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

  get ApplicationAsCopyrightRegistration () : CopyrightRegistrationViewModel
  {
    return (this.Application as CopyrightRegistrationViewModel);
  }

  get ApplicationAsOwnershipRestructure() : OwnershipRestructureViewModel
  {
    return (this.Application as OwnershipRestructureViewModel);
  }

  get ExistingWork(): RegisteredWorkViewModel
  {
    return <RegisteredWorkViewModel>this.warehouse.MyWorks.find(x => x.Id == this.Application.AssociatedWork?.Id);
  }

}
