import {Component, Input, OnInit} from '@angular/core';
import {CopyrightRegistrationViewModel} from "../../../_Models/Applications/CopyrightRegistrationViewModel";
import {ClrTimelineLayout, ClrTimelineStepState} from "@clr/angular";
import {CopyrightService} from "../../../_Services/copyright.service";
import {AlertService} from "../../../_Services/alert.service";
import {RegisteredWorkStatus} from "../../../_Models/Works/RegisteredWork";

@Component({
  selector: 'cpy-registration-view-submitted [Application]',
  templateUrl: './cpy-registration-view-submitted.component.html',
  styleUrls: ['./cpy-registration-view-submitted.component.css']
})
export class SubmittedViewComponent implements OnInit
{
  @Input() Application!: CopyrightRegistrationViewModel
  public timelineSuccess: ClrTimelineStepState = ClrTimelineStepState.SUCCESS;
  public timelineProcessing: ClrTimelineStepState = ClrTimelineStepState.PROCESSING;
  public timelineNot: ClrTimelineStepState = ClrTimelineStepState.NOT_STARTED;
  public vertical: ClrTimelineLayout = ClrTimelineLayout.VERTICAL;
  public timelineCurrent: ClrTimelineStepState = ClrTimelineStepState.CURRENT;

  constructor (private copyrightService: CopyrightService, private alertService: AlertService)
  {
  }

  ngOnInit (): void
  {
  }

  public Complete (): void
  {
    this.copyrightService.Complete(this.Application.Id).subscribe(x =>
    {
      this.alertService.Alert({Type: "info", Message: "Register transaction sent to the blockchain"});
      if (this.Application.AssociatedWork) this.Application.AssociatedWork.Status = RegisteredWorkStatus.SentToChain;
    });
  }
}
