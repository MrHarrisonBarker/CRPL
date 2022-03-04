import {Component, Inject, Input, OnInit} from '@angular/core';
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {ApplicationType} from "../../_Models/Applications/ApplicationViewModel";
import {ApplicationStatus} from "../../_Models/Applications/ApplicationStatus";
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";
import {CopyrightService} from "../../_Services/copyright.service";
import {syntaxHighlight} from "../../utils";
import {AlertService} from "../../_Services/alert.service";
import {WorksService} from "../../_Services/works.service";
import {finalize} from "rxjs/operators";

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

  private readonly BaseUrl;
  public Locked: boolean = false;

  constructor (
    private copyrightService: CopyrightService,
    private alertService: AlertService,
    private worksService: WorksService,
    @Inject('BASE_URL') baseUrl: string,)
  {
    this.BaseUrl = baseUrl;
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

  get ProxyLink (): string
  {
    return this.BaseUrl + 'proxy/cpy/' + this.Copyright.Id;
  }

  public Sync (): void
  {
    this.Locked = true;
    this.copyrightService.Sync(this.Copyright.Id)
        .pipe(finalize(() => this.Locked = false))
        .subscribe(x => this.alertService.Alert({
      Type: "info",
      Message: "The work has been synced with the blockchain"
    }), err => this.alertService.Alert({Type: 'danger', Message: 'There was an error syncing with the blockchain!'}));
  }
}
