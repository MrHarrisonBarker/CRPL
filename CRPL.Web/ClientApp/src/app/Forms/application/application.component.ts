import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {FormsService} from "../../_Services/forms.service";

@Component({
  selector: 'app-application',
  templateUrl: './application.component.html',
  styleUrls: ['./application.component.css']
})
export class ApplicationComponent implements OnInit
{

  public Application!: ApplicationViewModel;

  constructor (private route: ActivatedRoute, private formsService: FormsService)
  {
  }

  ngOnInit (): void
  {
    let applicationId = this.route.snapshot.paramMap.get('id');
    if (applicationId == null) null;// TODO: route away

    console.log(applicationId);

    if (applicationId != null)
    {
      this.formsService.GetApplication(applicationId).subscribe(x => this.Application = x);
    }
  }

  get ApplicationType ()
  {
    return this.Application?.ApplicationType.toString();
  }
}
