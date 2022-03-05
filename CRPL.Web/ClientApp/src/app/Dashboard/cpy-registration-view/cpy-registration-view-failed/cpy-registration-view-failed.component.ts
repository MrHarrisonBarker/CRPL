import {Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges} from '@angular/core';
import {CopyrightRegistrationViewModel} from "../../../_Models/Applications/CopyrightRegistrationViewModel";
import {Observable, Subscription} from "rxjs";
import {ApplicationViewModel} from "../../../_Models/Applications/ApplicationViewModel";

@Component({
  selector: 'cpy-registration-view-failed [ApplicationAsync]',
  templateUrl: './cpy-registration-view-failed.component.html',
  styleUrls: ['./cpy-registration-view-failed.component.css']
})
export class CpyRegistrationViewFailedComponent implements OnInit, OnChanges, OnDestroy
{
  @Input() ApplicationAsync!: Observable<ApplicationViewModel>;
  public Application!: CopyrightRegistrationViewModel;
  private ApplicationSubscription!: Subscription;

  constructor ()
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

  RouteToCollision ()
  {
    return ['/cpy', this.Application.AssociatedWork?.VerificationResult?.Collision];
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
