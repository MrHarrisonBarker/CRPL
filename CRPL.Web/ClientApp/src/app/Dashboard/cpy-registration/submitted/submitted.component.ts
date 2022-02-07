import {Component, Input, OnInit} from '@angular/core';
import {CopyrightRegistrationViewModel} from "../../../_Models/Applications/CopyrightRegistrationViewModel";

@Component({
  selector: 'cpy-registration-submitted [Application]',
  templateUrl: './submitted.component.html',
  styleUrls: ['./submitted.component.css']
})
export class SubmittedComponent implements OnInit
{
  @Input() Application!: CopyrightRegistrationViewModel

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

}
