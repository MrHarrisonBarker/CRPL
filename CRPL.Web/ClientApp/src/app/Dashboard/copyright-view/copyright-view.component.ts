import {Component, Inject, Input, OnChanges, OnDestroy, OnInit, SimpleChanges} from '@angular/core';
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {ApplicationType} from "../../_Models/Applications/ApplicationViewModel";
import {ApplicationStatus} from "../../_Models/Applications/ApplicationStatus";
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";
import {CopyrightService} from "../../_Services/copyright.service";
import {syntaxHighlight} from "../../utils";
import {AlertService} from "../../_Services/alert.service";
import {WorksService} from "../../_Services/works.service";
import {finalize} from "rxjs/operators";
import {Observable, Subscription} from "rxjs";

@Component({
  selector: 'copyright-view [CopyrightAsync]',
  templateUrl: './copyright-view.component.html',
  styleUrls: ['./copyright-view.component.css']
})
export class CopyrightViewComponent implements OnInit, OnChanges, OnDestroy
{
  @Input() CopyrightAsync!: Observable<RegisteredWorkViewModel>;
  public Copyright!: RegisteredWorkViewModel;
  private CopyrightSubscription!: Subscription;

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

  private subscribeToCopyright()
  {
    this.CopyrightSubscription = this.CopyrightAsync.subscribe(copyright => this.Copyright = copyright as RegisteredWorkViewModel);
  }

  ngOnInit (): void
  {
    if (this.CopyrightAsync) this.CopyrightSubscription = this.CopyrightAsync.subscribe(copyright =>
    {
      console.log("[copyright-view] got application", copyright);
      this.Copyright = copyright;
    });
  }

  get IsRestructureAllowed (): boolean
  {
    if (this.Copyright?.AssociatedApplication == null) return false;

    let openApplications = this.Copyright.AssociatedApplication.filter(x => x.ApplicationType == ApplicationType.OwnershipRestructure && x.Status == ApplicationStatus.Incomplete);
    let submittedApplications = this.Copyright.AssociatedApplication.find(x => x.ApplicationType == ApplicationType.OwnershipRestructure && x.Status == ApplicationStatus.Submitted);

    return !submittedApplications;
  }

  get ExistingRestructure (): OwnershipRestructureViewModel
  {
    if (this.Copyright?.AssociatedApplication)
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
    if (this.Copyright?.RightId) this.copyrightService.BindProposalWork({
      WorkId: this.Copyright.Id,
      Accepted: true
    }).subscribe();
  }

  public RejectRestructure (): void
  {
    if (this.Copyright?.RightId) this.copyrightService.BindProposalWork({
      WorkId: this.Copyright.Id,
      Accepted: false
    }).subscribe();
  }

  get Meta ()
  {
    return syntaxHighlight(JSON.stringify(this.Copyright?.Meta, undefined, 4));
  }

  get ProxyLink (): string
  {
    return this.BaseUrl + 'proxy/cpy/' + this.Copyright?.Id;
  }

  public Sync (): void
  {
    this.Locked = true;
    if (this.Copyright)
    {
      this.copyrightService.Sync(this.Copyright?.Id)
          .pipe(finalize(() => this.Locked = false))
          .subscribe(x => this.alertService.Alert({
            Type: "info",
            Message: "The work has been synced with the blockchain"
          }), err => this.alertService.Alert({
            Type: 'danger',
            Message: 'There was an error syncing with the blockchain!'
          }));
    }
  }

  ngOnChanges (changes: SimpleChanges): void
  {
    if (this.CopyrightSubscription)
    {
      this.CopyrightSubscription.unsubscribe();
      this.subscribeToCopyright();
    }
  }

  ngOnDestroy (): void
  {
    if (this.CopyrightSubscription) this.CopyrightSubscription.unsubscribe();
  }
}
