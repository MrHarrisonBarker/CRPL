import {Component, Input, OnInit} from '@angular/core';
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {ApplicationType} from "../../_Models/Applications/ApplicationViewModel";
import {ApplicationStatus} from "../../_Models/Applications/ApplicationStatus";
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";
import {CopyrightService} from "../../_Services/copyright.service";
import {syntaxHighlight} from "../../utils";

@Component({
  selector: 'copyright-view [Copyright]',
  templateUrl: './copyright-view.component.html',
  styleUrls: ['./copyright-view.component.css']
})
export class CopyrightViewComponent implements OnInit
{
  @Input() Copyright!: RegisteredWorkViewModel;
  public RestructureOpen: boolean = false;

  constructor (private copyrightService: CopyrightService)
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

    return  !submittedApplications;
  }

  get ExistingRestructure (): OwnershipRestructureViewModel
  {
    if (this.Copyright.AssociatedApplication)
    {
      return this.Copyright.AssociatedApplication.find(x => x.ApplicationType == ApplicationType.OwnershipRestructure && x.Status == ApplicationStatus.Incomplete) as OwnershipRestructureViewModel;
    }
    return undefined as any;
  }

  public CancelRestructure (): void
  {
    // TODO: send cancel call
  }

  public BindRestructure (): void
  {
    if (this.Copyright.RightId) this.copyrightService.BindProposalWork({WorkId: this.Copyright.Id, Accepted: true}).subscribe();
  }

  public RejectRestructure (): void
  {
    if (this.Copyright.RightId) this.copyrightService.BindProposalWork({WorkId: this.Copyright.Id, Accepted: false}).subscribe();
  }

  get Meta()
  {
    return syntaxHighlight(JSON.stringify(this.Copyright.Meta, undefined, 4));
  }
}
