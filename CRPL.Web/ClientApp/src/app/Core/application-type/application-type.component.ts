import {Component, Input, OnInit} from '@angular/core';
import {ApplicationType, ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";

@Component({
  selector: 'application-type [Application]',
  template: '<span [ngClass]="map[Type]">{{Type}}</span>',
  styleUrls: ['']
})
export class ApplicationTypeComponent implements OnInit
{
  @Input() Application!: ApplicationViewModel;
  map: Record<string, string> = {
    CopyrightRegistration: "badge",
    OwnershipRestructure: "badge badge-light-blue",
    CopyrightTypeChange: "badge badge-purple",
    Dispute: "badge badge-4"
  }

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

  get Type(): string
  {
    return ApplicationType[this.Application.ApplicationType];
  }

}
