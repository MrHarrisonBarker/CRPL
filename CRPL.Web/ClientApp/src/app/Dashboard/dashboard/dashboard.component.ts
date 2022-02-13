import {Component, OnInit} from '@angular/core';
import {FormsService} from "../../_Services/forms.service";
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {CopyrightService} from "../../_Services/copyright.service";
import {RegisteredWorkStatus, RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {ApplicationStatus} from "../../_Models/Applications/ApplicationStatus";
import {forkJoin} from "rxjs";
import {AlertService} from "../../_Services/alert.service";
import {WarehouseService} from "../../_Services/warehouse.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit
{
  public Loaded: boolean = false;
  public Selected!: ApplicationViewModel | RegisteredWorkViewModel;
  public IsApplication: boolean = false;

  constructor (
    private formsService: FormsService,
    private copyrightService: CopyrightService,
    private alertService: AlertService,
    private warehouse: WarehouseService,
    private route: ActivatedRoute)
  {
  }

  async ngOnInit (): Promise<any>
  {
    this.alertService.StartLoading();
    // GET EVERYTHING
    await forkJoin([this.formsService.GetMyApplications(), this.copyrightService.GetMyCopyrights()]).subscribe(x =>
    {
      console.log(this.warehouse.MyApplications, this.warehouse.MyWorks);

      // ROUTE WORK
      if (this.route.snapshot.paramMap.has("workId")) this.Selected = this.warehouse.MyWorks.find(x => x.Id == this.route.snapshot.paramMap.get("workId")) as RegisteredWorkViewModel

      // ROUTE APPLICATION
      if (this.route.snapshot.paramMap.has("applicationId")) this.Selected = this.warehouse.MyApplications.find(x => x.Id == this.route.snapshot.paramMap.get("applicationId")) as ApplicationViewModel

      this.Loaded = true;
      this.alertService.StopLoading();
    });

    console.log(this.route.snapshot.paramMap.keys);

  }

  get RegisteredCopyrights (): RegisteredWorkViewModel[]
  {
    return this.warehouse.MyWorks.filter(x => x.Status == RegisteredWorkStatus.Registered);
  }

  get PartialApplications (): ApplicationViewModel[]
  {
    return this.warehouse.MyApplications.filter(x => x.Status == ApplicationStatus.Incomplete);
  }

  get CompletedApplications (): ApplicationViewModel[]
  {
    return this.warehouse.MyApplications.filter(x => x.Status == ApplicationStatus.Complete);
  }

  get SubmittedApplications (): ApplicationViewModel[]
  {
    return this.warehouse.MyApplications.filter(x => x.Status == ApplicationStatus.Submitted);
  }

  get SelectedAsCopyright () : RegisteredWorkViewModel
  {
    return (this.Selected as RegisteredWorkViewModel);
  }

  get SelectedAsApplication () : ApplicationViewModel
  {
    return (this.Selected as ApplicationViewModel);
  }

  public Select (selected: ApplicationViewModel | RegisteredWorkViewModel, isApplication: boolean): void
  {
    this.Selected = selected;
    this.IsApplication = isApplication;
  }
}
