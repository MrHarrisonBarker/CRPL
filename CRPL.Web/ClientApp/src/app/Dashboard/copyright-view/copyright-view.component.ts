import {Component, Input, OnInit} from '@angular/core';
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {ApplicationType} from "../../_Models/Applications/ApplicationViewModel";
import {ApplicationStatus} from "../../_Models/Applications/ApplicationStatus";
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";

@Component({
  selector: 'copyright-view [Copyright]',
  templateUrl: './copyright-view.component.html',
  styleUrls: ['./copyright-view.component.css']
})
export class CopyrightViewComponent implements OnInit
{
  @Input() Copyright!: RegisteredWorkViewModel;
  public RestructureOpen: boolean = false;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

  get IsRestructureAllowed (): boolean
  {
    if (this.Copyright.AssociatedApplication == null) return false;

    let openApplications = this.Copyright.AssociatedApplication.filter(x => x.ApplicationType == ApplicationType.OwnershipRestructure && x.Status == ApplicationStatus.Incomplete);
    let submittedApplications = this.Copyright.AssociatedApplication.find(x => x.ApplicationType == ApplicationType.OwnershipRestructure && x.Status == ApplicationStatus.Submitted);

    return openApplications != undefined || !submittedApplications;
  }

  get ExistingRestructure (): OwnershipRestructureViewModel | undefined
  {
    if (this.Copyright.AssociatedApplication)
    {
      return this.Copyright.AssociatedApplication.find(x => x.ApplicationType == ApplicationType.OwnershipRestructure && x.Status != ApplicationStatus.Complete) as OwnershipRestructureViewModel;
    }
    return undefined as any;
  }

}
