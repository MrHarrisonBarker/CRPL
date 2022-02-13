import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {CopyrightService} from "../_Services/copyright.service";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {ClarityIcons, undoIcon} from "@cds/core/icon";
import {Location} from "@angular/common";

@Component({
  selector: 'app-copyright',
  templateUrl: './copyright.component.html',
  styleUrls: ['./copyright.component.css']
})
export class CopyrightComponent implements OnInit
{
  public Copyright!: RegisteredWorkViewModel;

  constructor (private route: ActivatedRoute, private copyrightService: CopyrightService, private location: Location)
  {
    ClarityIcons.addIcons(undoIcon);
  }

  async ngOnInit (): Promise<any>
  {
    let workId = this.route.snapshot.paramMap.get('id');
    if (workId) await this.copyrightService.Get(workId).subscribe(x => this.Copyright = x);
  }

  Back ()
  {
    this.location.back();
  }
}
