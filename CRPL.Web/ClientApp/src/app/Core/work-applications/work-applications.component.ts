import {Component, Input, OnInit} from '@angular/core';
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";

@Component({
  selector: 'work-applications [Copyright]',
  templateUrl: './work-applications.component.html',
  styleUrls: ['./work-applications.component.css']
})
export class WorkApplicationsComponent implements OnInit
{
  @Input() Copyright!: RegisteredWorkViewModel;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

  public routeToApplication (Id: string)
  {
    return ['/dashboard', {applicationId: Id}];
  }

}
