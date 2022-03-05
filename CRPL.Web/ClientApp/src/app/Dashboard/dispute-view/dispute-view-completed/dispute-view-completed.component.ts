import {Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges} from '@angular/core';
import {DisputeType} from 'src/app/_Models/Applications/DisputeInputModel';
import {DisputeViewModel, ExpectedRecourse} from "../../../_Models/Applications/DisputeViewModel";
import {Observable, Subscription} from "rxjs";
import {ApplicationViewModel} from "../../../_Models/Applications/ApplicationViewModel";

@Component({
  selector: 'dispute-view-completed [ApplicationAsync]',
  templateUrl: './dispute-view-completed.component.html',
  styleUrls: ['./dispute-view-completed.component.css']
})
export class DisputeViewCompletedComponent implements OnInit, OnChanges, OnDestroy
{
  @Input() ApplicationAsync!: Observable<ApplicationViewModel>;
  public Application!: DisputeViewModel;
  private ApplicationSubscription!: Subscription;

  constructor ()
  {
  }

  private subscribeToApplication()
  {
    this.ApplicationSubscription = this.ApplicationAsync.subscribe(application => this.Application = application as DisputeViewModel);
  }

  ngOnInit (): void
  {
    this.subscribeToApplication();
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

  get DisputeType ()
  {
    return DisputeType[this.Application.DisputeType];
  }

  get ExpectedRecourse()
  {
    return ExpectedRecourse[this.Application.ExpectedRecourse];
  }
}
