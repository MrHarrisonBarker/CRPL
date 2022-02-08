import {Component, OnInit} from '@angular/core';
import {FormsService} from "../../_Services/forms.service";
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {CopyrightService} from "../../_Services/copyright.service";
import {RegisteredWorkStatus, RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {ApplicationStatus} from "../../_Models/Applications/ApplicationStatus";
import {forkJoin} from "rxjs";
import {AlertService} from "../../_Services/alert.service";
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit
{

  public MyApplications!: ApplicationViewModel[];
  public Works!: RegisteredWorkViewModel[];
  public Loaded: boolean = false;
  public Selected!: ApplicationViewModel | RegisteredWorkViewModel;
  public IsApplication: boolean = false;

  constructor (private formsService: FormsService, private copyrightService: CopyrightService, private alertService: AlertService)
  {
  }

  ngOnInit (): void
  {
    this.alertService.StartLoading();
    forkJoin([this.formsService.GetMyApplications(), this.copyrightService.GetMyCopyrights()]).subscribe(x =>
    {
      this.MyApplications = x[0];
      this.Works = x[1];
      console.log(this.MyApplications, this.Works);
      this.Loaded = true;
      this.alertService.StopLoading();
    });
  }

  get RegisteredCopyrights (): RegisteredWorkViewModel[]
  {
    return this.Works.filter(x => x.Status == RegisteredWorkStatus.Registered);
  }

  get PartialApplications (): ApplicationViewModel[]
  {
    return this.MyApplications.filter(x => x.Status == ApplicationStatus.Incomplete);
  }

  get CompletedApplications (): ApplicationViewModel[]
  {
    return this.MyApplications.filter(x => x.Status == ApplicationStatus.Complete);
  }

  get SubmittedApplications (): ApplicationViewModel[]
  {
    return this.MyApplications.filter(x => x.Status == ApplicationStatus.Submitted);
  }

  get SelectedAsCopyright ()
  {
    return (this.Selected as RegisteredWorkViewModel);
  }

  public Select (selected: ApplicationViewModel | RegisteredWorkViewModel, isApplication: boolean): void
  {
    this.Selected = selected;
    this.IsApplication = isApplication;
  }
}
