import {Component, Input, OnInit} from '@angular/core';
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";
import {WorkType} from "../../_Models/WorkType";
import {ClrTimelineStepState} from "@clr/angular";
import {syntaxHighlight} from "../../utils";

@Component({
  selector: 'cpy-registration-view [Application]',
  templateUrl: './cpy-registration-view.component.html',
  styleUrls: ['./cpy-registration-view.component.css']
})
export class CpyRegistrationViewComponent implements OnInit
{
  @Input() Application!: CopyrightRegistrationViewModel;
  public timelineSuccess: ClrTimelineStepState = ClrTimelineStepState.SUCCESS;
  public timelineProcessing: ClrTimelineStepState = ClrTimelineStepState.PROCESSING;
  public timelineNot: ClrTimelineStepState = ClrTimelineStepState.NOT_STARTED;
  public timelineCurrent: ClrTimelineStepState = ClrTimelineStepState.CURRENT;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

  get Meta()
  {
    let meta = {
      Title: this.Application.Title,
      WorkType: WorkType[this.Application.WorkType],
      WorkHash: this.Application.WorkHash,
      WorkUri: this.Application.WorkUri,
      Legal: this.Application.Legal,
      OwnershipStructure: this.Application.OwnershipStakes,
      ProtectionLength: this.Application.YearsExpire
    };

    return syntaxHighlight(JSON.stringify(meta, undefined, 4));
  }

  get WorkStatus()
  {
    return this.Application.AssociatedWork?.Status;
  }
}


