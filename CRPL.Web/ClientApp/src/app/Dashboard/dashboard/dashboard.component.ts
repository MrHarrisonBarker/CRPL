import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormsService} from "../../_Services/forms.service";
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {CopyrightService} from "../../_Services/copyright.service";
import {RegisteredWorkStatus, RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {ApplicationStatus} from "../../_Models/Applications/ApplicationStatus";
import {forkJoin, Observable} from "rxjs";
import {AlertService} from "../../_Services/alert.service";
import {WarehouseService} from "../../_Services/warehouse.service";
import {ActivatedRoute} from "@angular/router";
import {map} from "rxjs/operators";
import {DisputeViewModel} from "../../_Models/Applications/DisputeViewModel";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy
{
  public Loaded: boolean = false;
  public Selected!: ApplicationViewModel | RegisteredWorkViewModel;
  public IsApplication: boolean = false;

  public CompletedApplications!: Observable<ApplicationViewModel[]>;
  public SubmittedApplications!: Observable<ApplicationViewModel[]>;
  public PartialApplications!: Observable<ApplicationViewModel[]>;
  public FailedApplications!: Observable<ApplicationViewModel[]>;

  public OpenDisputes!: Observable<DisputeViewModel[]>;
  public ClosedDisputes!: Observable<DisputeViewModel[]>;

  public RegisteredCopyrights!: Observable<RegisteredWorkViewModel[]>;

  constructor (
    private formsService: FormsService,
    private copyrightService: CopyrightService,
    private alertService: AlertService,
    public warehouse: WarehouseService,
    private route: ActivatedRoute)
  {
    this.CompletedApplications = this.warehouse.__MyApplications.pipe(map(x => x.filter(a => a.Status == ApplicationStatus.Complete)));
    this.SubmittedApplications = this.warehouse.__MyApplications.pipe(map(x => x.filter(a => a.Status == ApplicationStatus.Submitted)));
    this.PartialApplications = this.warehouse.__MyApplications.pipe(map(x => x.filter(a => a.Status == ApplicationStatus.Incomplete)));
    this.FailedApplications = this.warehouse.__MyApplications.pipe(map(x => x.filter(a => a.Status == ApplicationStatus.Failed)));

    this.OpenDisputes = this.warehouse.__MyDisputed.pipe(map(x => x.filter(a => a.Status == ApplicationStatus.Submitted)));
    this.ClosedDisputes = this.warehouse.__MyDisputed.pipe(map(x => x.filter(a => a.Status == ApplicationStatus.Complete || a.Status == ApplicationStatus.Failed)));

    this.RegisteredCopyrights = this.warehouse.__MyWorks.pipe(map(x => x.filter(a => a.Status == RegisteredWorkStatus.Registered || a.Status == RegisteredWorkStatus.Expired)));
  }

  async ngOnInit (): Promise<any>
  {

    // GET EVERYTHING
    await forkJoin([this.formsService.GetMyApplications(), this.copyrightService.GetMyCopyrights(), this.copyrightService.GetMyDisputed()]).subscribe(async x =>
    {
      this.Loaded = true;
    });

    // ROUTE WORK
    if (this.route.snapshot.paramMap.has("workId"))
    {
      await this.warehouse.__MyWorks.pipe(map(w => w.find(x => x.Id == this.route.snapshot.paramMap.get("workId")))).subscribe(x => this.Selected = x as RegisteredWorkViewModel);
    }

    // ROUTE APPLICATION
    if (this.route.snapshot.paramMap.has("applicationId"))
    {
      await this.warehouse.__MyApplications.pipe(map(w => w.find(x => x.Id == this.route.snapshot.paramMap.get("applicationId")))).subscribe(x => this.Selected = x as ApplicationViewModel);
      this.IsApplication = true;
    }

    console.log(this.route.snapshot.paramMap.keys);
  }

  public ngOnDestroy (): void
  {
  }

  get SelectedAsCopyright (): RegisteredWorkViewModel
  {
    return (this.Selected as RegisteredWorkViewModel);
  }

  get SelectedAsApplication (): ApplicationViewModel
  {
    return (this.Selected as ApplicationViewModel);
  }

  public Select (selected: ApplicationViewModel | RegisteredWorkViewModel, isApplication: boolean): void
  {
    console.log("selected", selected);
    this.Selected = selected;
    this.IsApplication = isApplication;
  }

  public NumberOfOpenDisputes (right: RegisteredWorkViewModel): number
  {
    if (right.AssociatedApplication) return right.AssociatedApplication?.filter(x => x.ApplicationType == 2 && x.Status != ApplicationStatus.Complete).length;
    return 0;
  }

  public ResetSelected ()
  {
    this.Selected = null as any;
  }
}
