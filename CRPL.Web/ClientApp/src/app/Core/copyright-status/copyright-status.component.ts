import {Component, Input, OnInit} from '@angular/core';
import {RegisteredWorkStatus} from "../../_Models/Works/RegisteredWork";

@Component({
  selector: ' copyright-status [status]',
  template: '<span [ngClass]="map[Status]">{{Status}}</span>',
  styles: ['']
})
export class CopyrightStatusComponent implements OnInit
{
  @Input() status!: number;
  map: Record<string, string> = {
    Created: "label label-purple",
    ProcessingVerification: "label label-blue",
    Verified: "label label-blue",
    SentToChain: "label label-blue",
    Registered: "label label-success",
    Rejected: "label label-blue",
    Expired: "label label-orange",
  }

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

  get Status() : string
  {
    return RegisteredWorkStatus[this.status];
  }
}
