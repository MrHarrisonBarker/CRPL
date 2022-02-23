import {Component, Input, OnInit} from '@angular/core';
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {ApplicationType} from "../../_Models/Applications/ApplicationViewModel";
import {ApplicationStatus} from "../../_Models/Applications/ApplicationStatus";
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";
import {CopyrightService} from "../../_Services/copyright.service";
import {DownloadFile, syntaxHighlight} from "../../utils";
import {AlertService} from "../../_Services/alert.service";
import {WorksService} from "../../_Services/works.service";

@Component({
  selector: 'copyright-view [Copyright]',
  templateUrl: './copyright-view.component.html',
  styleUrls: ['./copyright-view.component.css']
})
export class CopyrightViewComponent implements OnInit
{
  @Input() Copyright!: RegisteredWorkViewModel;
  @Input() ShowActions: boolean = true;
  public RestructureOpen: boolean = false;

  constructor (
    private copyrightService: CopyrightService,
    private alertService: AlertService,
    private worksService: WorksService)
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

    return !submittedApplications;
  }

  public CancelRestructure (): void
  {
    // TODO: send cancel call
  }

  public BindRestructure (): void
  {
    if (this.Copyright.RightId) this.copyrightService.BindProposalWork({
      WorkId: this.Copyright.Id,
      Accepted: true
    }).subscribe();
  }

  public RejectRestructure (): void
  {
    if (this.Copyright.RightId) this.copyrightService.BindProposalWork({
      WorkId: this.Copyright.Id,
      Accepted: false
    }).subscribe();
  }

  get Meta ()
  {
    return syntaxHighlight(JSON.stringify(this.Copyright.Meta, undefined, 4));
  }

  public Sync (): void
  {
    this.copyrightService.Sync(this.Copyright.Id).subscribe(x => this.alertService.Alert({
      Type: "info",
      Message: "The work has been synced with the blockchain"
    }));
  }

  public Download (): void
  {
    this.worksService.GetSignedWork(this.Copyright.Id).subscribe(data =>
    {
      DownloadFile(this.Copyright.Meta?.Title + "-master", data);
      console.log(data);
    });
  }
}
