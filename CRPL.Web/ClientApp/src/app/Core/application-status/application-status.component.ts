import {Component, Input, OnInit} from '@angular/core';
import {ApplicationStatus} from "../../_Models/Applications/ApplicationStatus";

@Component({
  selector: 'application-status [status]',
  template: '<span [ngClass]="map[Status]">{{Status}}</span>',
  styles: ['']
})
export class ApplicationStatusComponent implements OnInit
{
  @Input() status!: number;
  map: Record<string, string> = {
    Complete: "badge badge-success",
    Incomplete: "badge badge-danger",
    Submitted: "badge badge-purple",
  }

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

  get Status() : string
  {
    return ApplicationStatus[this.status];
  }

}
