import {Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges} from '@angular/core';
import {CopyrightRegistrationViewModel} from "../../../_Models/Applications/CopyrightRegistrationViewModel";
import {ClrTimelineLayout, ClrTimelineStepState} from "@clr/angular";
import {CopyrightService} from "../../../_Services/copyright.service";
import {AlertService} from "../../../_Services/alert.service";
import {RegisteredWorkStatus} from "../../../_Models/Works/RegisteredWork";
import {finalize} from "rxjs/operators";
import {Observable, Subscription} from "rxjs";
import {ApplicationViewModel} from "../../../_Models/Applications/ApplicationViewModel";

@Component({
  selector: 'cpy-registration-view-submitted [ApplicationAsync]',
  templateUrl: './cpy-registration-view-submitted.component.html',
  styleUrls: ['./cpy-registration-view-submitted.component.css']
})
export class SubmittedViewComponent implements OnInit, OnDestroy, OnChanges
{
  @Input() ApplicationAsync!: Observable<ApplicationViewModel>;
  public Application!: CopyrightRegistrationViewModel;
  private ApplicationSubscription!: Subscription;

  public timelineSuccess: ClrTimelineStepState = ClrTimelineStepState.SUCCESS;
  public timelineProcessing: ClrTimelineStepState = ClrTimelineStepState.PROCESSING;
  public timelineNot: ClrTimelineStepState = ClrTimelineStepState.NOT_STARTED;
  public vertical: ClrTimelineLayout = ClrTimelineLayout.VERTICAL;
  public timelineCurrent: ClrTimelineStepState = ClrTimelineStepState.CURRENT;
  public Locked: boolean = false;

  constructor (private copyrightService: CopyrightService, private alertService: AlertService)
  {

  }

  private subscribeToApplication()
  {
    this.ApplicationSubscription = this.ApplicationAsync.subscribe(application => this.Application = application as CopyrightRegistrationViewModel);
  }

  ngOnInit (): void
  {
    this.subscribeToApplication();
  }

  public Complete (): void
  {
    this.Locked = true;
    this.copyrightService.Complete(this.Application.Id)
        .pipe(finalize(() => this.Locked = false))
        .subscribe(x =>
        {
          this.alertService.Alert({Type: "info", Message: "Register transaction sent to the blockchain"});
          if (this.Application.AssociatedWork) this.Application.AssociatedWork.Status = RegisteredWorkStatus.SentToChain;
        }, error => this.alertService.Alert({Type: 'danger', Message: 'There was an error sending transaction'}));
  }

  ngOnChanges (changes: SimpleChanges): void
  {
    if (this.ApplicationSubscription)
    {
      this.ApplicationSubscription.unsubscribe();
      this.subscribeToApplication();
    }
  }

  ngOnDestroy (): void
  {
    if (this.ApplicationSubscription) this.ApplicationSubscription.unsubscribe();
  }
}
