import {Component, Input, OnInit} from '@angular/core';
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";
import {CopyrightService} from "../../_Services/copyright.service";

@Component({
  selector: 'cpy-restructure-view [Application]',
  templateUrl: './cpy-restructure-view.component.html',
  styleUrls: ['./cpy-restructure-view.component.css']
})
export class CpyRestructureViewComponent implements OnInit
{
  @Input() Application!: OwnershipRestructureViewModel

  constructor (private copyrightService: CopyrightService)
  {
  }

  ngOnInit (): void
  {
  }

  Cancel ()
  {

  }

  Bind ()
  {
    this.copyrightService.BindProposal({
      ApplicationId: this.Application.Id,
      Accepted: true
    }).subscribe(x => console.log(x));
  }
}
