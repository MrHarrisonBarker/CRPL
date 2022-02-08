import {Component, Input, OnInit} from '@angular/core';
import {CopyrightRegistrationViewModel} from "../../../_Models/Applications/CopyrightRegistrationViewModel";
import {ClrTimelineStepState} from "@clr/angular";

@Component({
  selector: 'cpy-registration-submitted [Application]',
  templateUrl: './submitted.component.html',
  styleUrls: ['./submitted.component.css']
})
export class SubmittedComponent implements OnInit
{
  @Input() Application!: CopyrightRegistrationViewModel
  public timelineSuccess: ClrTimelineStepState = ClrTimelineStepState.SUCCESS;
  public timelineProcessing: ClrTimelineStepState = ClrTimelineStepState.PROCESSING;
  public timelineNot: ClrTimelineStepState = ClrTimelineStepState.NOT_STARTED;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

}
