import {Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges} from '@angular/core';
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";
import {Observable, Subscription} from "rxjs";
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";

@Component({
  selector: 'cpy-restructure-view [ApplicationAsync]',
  templateUrl: './cpy-restructure-view.component.html',
  styleUrls: ['./cpy-restructure-view.component.css']
})
export class CpyRestructureViewComponent implements OnInit, OnChanges, OnDestroy
{
  @Input() ApplicationAsync!: Observable<ApplicationViewModel>;
  public Application!: OwnershipRestructureViewModel;
  private ApplicationSubscription!: Subscription;

  constructor ()
  {
  }

  private subscribeToApplication()
  {
    this.ApplicationSubscription = this.ApplicationAsync.subscribe(application => {
      console.log("[cpy-registration-view] got registration application", application);
      this.Application = application as OwnershipRestructureViewModel;
    });
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
}
