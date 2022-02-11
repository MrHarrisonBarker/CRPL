import {Component, Input, OnInit} from '@angular/core';
import {ApplicationType, ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";

@Component({
  selector: 'application-type [Application]',
  template: '<span [ngClass]="map[Type]">{{Type}}</span>',
  styles: ['']
})
export class ApplicationTypeComponent implements OnInit
{
  @Input() Application!: ApplicationViewModel;
  map: Record<string, string> = {
    CopyrightRegistration: "label",
    OwnershipRestructure: "label label-purple",
    CopyrightTypeChange: "label label-purple",
    Dispute: "label label-danger"
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
