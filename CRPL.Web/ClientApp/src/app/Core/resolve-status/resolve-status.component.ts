import {Component, Input, OnInit} from '@angular/core';
import {ResolvedStatus, ResolveResult} from "../../_Models/Applications/DisputeViewModel";

@Component({
  selector: 'resolve-status [Resolve]',
  template: '<span [ngClass]="map[Status]">{{Status}}</span>'
})
export class ResolveStatusComponent implements OnInit
{
  @Input() Resolve!: ResolveResult
  map: Record<string, string> = {
    Created: "label label-purple",
    NeedsOnChainAction: "label label-blue",
    Processing: "label label-blue",
    Resolved: "label label-success",
    Failed: "label label-danger",
  }

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

  get Status (): string
  {
    return ResolvedStatus[this.Resolve.ResolvedStatus];
  }

}
