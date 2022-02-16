import {Component, Input, OnInit} from '@angular/core';
import {CopyrightRegistrationViewModel} from "../../../_Models/Applications/CopyrightRegistrationViewModel";

@Component({
  selector: 'cpy-registration-view-failed [Application]',
  templateUrl: './cpy-registration-view-failed.component.html',
  styleUrls: ['./cpy-registration-view-failed.component.css']
})
export class CpyRegistrationViewFailedComponent implements OnInit
{

  @Input() Application!: CopyrightRegistrationViewModel;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

  RouteToCollision ()
  {
    return ['/cpy', this.Application.AssociatedWork?.VerificationResult?.Collision];
  }
}
