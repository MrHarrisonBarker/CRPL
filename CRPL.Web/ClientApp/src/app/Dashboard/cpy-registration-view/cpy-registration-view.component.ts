import {Component, Input, OnInit} from '@angular/core';
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";

@Component({
  selector: 'cpy-registration-view [Application]',
  templateUrl: './cpy-registration-view.component.html',
  styleUrls: ['./cpy-registration-view.component.css']
})
export class CpyRegistrationViewComponent implements OnInit
{
  @Input() Application!: CopyrightRegistrationViewModel;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

}
