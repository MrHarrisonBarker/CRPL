import {Component, Input, OnInit} from '@angular/core';
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";

@Component({
  selector: 'cpy-registration-view [Application]',
  templateUrl: './cpy-registration.component.html',
  styleUrls: ['./cpy-registration.component.css']
})
export class CpyRegistrationComponentView implements OnInit
{
  @Input() Application!: CopyrightRegistrationViewModel;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

}
