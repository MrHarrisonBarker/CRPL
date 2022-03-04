import {Component, Input, OnInit} from '@angular/core';
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";
import {WarehouseService} from "../../_Services/warehouse.service";
import {AlertService} from "../../_Services/alert.service";
import {FormsService} from "../../_Services/forms.service";
import {ClarityIcons, trashIcon} from "@cds/core/icon";
import {DisputeViewModel} from "../../_Models/Applications/DisputeViewModel";
import {finalize} from "rxjs/operators";

@Component({
  selector: 'applications-view [Application] [ShowForms]',
  templateUrl: './applications-view.component.html',
  styleUrls: ['./applications-view.component.css']
})
export class ApplicationsViewComponent implements OnInit
{
  @Input() Application!: ApplicationViewModel;
  @Input() ShowForms: boolean = false;
  @Input() Cancelable: boolean = true;
  public Locked: boolean = false;

  constructor (private warehouse: WarehouseService, private alertService: AlertService, private formsService: FormsService)
  {
    ClarityIcons.addIcons(trashIcon);
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

  get ApplicationAsCopyrightRegistration (): CopyrightRegistrationViewModel
  {
    return (this.Application as CopyrightRegistrationViewModel);
  }

  get ApplicationAsOwnershipRestructure (): OwnershipRestructureViewModel
  {
    return (this.Application as OwnershipRestructureViewModel);
  }

  get ApplicationAsDispute (): DisputeViewModel
  {
    return (this.Application as DisputeViewModel);
  }

  get ExistingWork (): RegisteredWorkViewModel
  {
    return <RegisteredWorkViewModel>this.warehouse.MyWorks.find(x => x.Id == this.Application.AssociatedWork?.Id);
  }

  public Cancel (): void
  {
    this.Locked = true;
    this.formsService.Cancel(this.Application.Id)
        .pipe(finalize(() => this.Locked = false))
        .subscribe(x =>
        {
          this.alertService.Alert({Type: 'success', Message: 'Canceled application'});
          this.Application = null as any;
          this.alertService.TriggerChange.emit();
        }, error => this.alertService.Alert({
          Type: 'danger',
          Message: 'There was a problem when canceling this application'
        }));

  }
}
