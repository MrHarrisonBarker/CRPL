import {Component, Input, OnInit} from '@angular/core';
import {RegisteredWorkStatus} from "../../_Models/Works/RegisteredWork";

@Component({
  selector: 'copyright-status [status]',
  template: '<span [ngClass]="map[Status]">{{Status}}</span>',
  styles: ['']
})
export class CopyrightStatusComponent implements OnInit
{
  @Input() status!: number;
  map: Record<string, string> = {
    Registered: "badge badge-success",
    Verified: "badge badge-blue",
    Created: "badge badge-purple",
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