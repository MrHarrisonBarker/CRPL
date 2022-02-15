import {Component, Input, OnInit} from '@angular/core';
import {BindStatus} from "../../_Models/Applications/OwnershipRestructureViewModel";

@Component({
  selector: 'bind-status [status]',
  template: '<span [ngClass]="map[Status]">{{Status}}</span>',
  styles: ['']
})
export class BindStatusComponent implements OnInit
{
  @Input() status!: number;
  map: Record<string, string> = {
    NoProposal: "label label-purple",
    AwaitingVotes: "label label-blue",
    Bound: "label label-success",
    Rejected: "label label-danger",
  }

  constructor ()
  {
  }

  ngOnInit (): void
  {

  }

  get Status (): string
  {
    return BindStatus[this.status];
  }
}
