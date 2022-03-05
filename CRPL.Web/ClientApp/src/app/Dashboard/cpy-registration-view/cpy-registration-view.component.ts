import {Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges} from '@angular/core';
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";
import {WorkType} from "../../_Models/WorkType";
import {ClrTimelineStepState} from "@clr/angular";
import {syntaxHighlight} from "../../utils";
import {Observable, Subscription} from "rxjs";
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";

@Component({
  selector: 'cpy-registration-view [ApplicationAsync]',
  templateUrl: './cpy-registration-view.component.html',
  styleUrls: ['./cpy-registration-view.component.css']
})
export class CpyRegistrationViewComponent implements OnInit, OnChanges, OnDestroy
{
  @Input() ApplicationAsync!: Observable<ApplicationViewModel>;
  public Application!: CopyrightRegistrationViewModel;
  private ApplicationSubscription!: Subscription;

  public timelineSuccess: ClrTimelineStepState = ClrTimelineStepState.SUCCESS;
  public timelineProcessing: ClrTimelineStepState = ClrTimelineStepState.PROCESSING;
  public timelineNot: ClrTimelineStepState = ClrTimelineStepState.NOT_STARTED;
  public timelineCurrent: ClrTimelineStepState = ClrTimelineStepState.CURRENT;

  constructor ()
  {
  }

  private subscribeToApplication()
  {
    this.ApplicationSubscription = this.ApplicationAsync.subscribe(application => {
      console.log("[cpy-registration-view] got registration application", application);
      this.Application = application as CopyrightRegistrationViewModel;
    });
  }

  ngOnInit (): void
  {
    this.subscribeToApplication();
  }

  get Meta()
  {
    let meta = {
      Title: this.Application.Title,
      WorkType: WorkType[this.Application.WorkType],
      YearsOfProtection: this.Application.YearsExpire,
      WorkHash: this.Application.WorkHash,
      WorkUri: this.Application.WorkUri,
      Legal: this.Application.Legal,
      OwnershipStructure: this.Application.OwnershipStakes,
      ProtectionLength: this.Application.YearsExpire,
      Protections: this.Application.Protections,
      AssociatedWork: this.Application.AssociatedWork
    };

    return syntaxHighlight(JSON.stringify(meta, undefined, 4));
  }

  get WorkStatus()
  {
    return this.Application.AssociatedWork?.Status;
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


