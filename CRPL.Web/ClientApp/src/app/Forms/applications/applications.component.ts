import {Component, OnInit} from '@angular/core';
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {FormsService} from "../../_Services/forms.service";

@Component({
  selector: 'app-applications',
  templateUrl: './applications.component.html',
  styleUrls: ['./applications.component.css']
})
export class ApplicationsComponent implements OnInit
{
  public selected: string = "reg";
  public Applications!: ApplicationViewModel[];

  constructor (private formsService: FormsService)
  {
  }

  ngOnInit (): void
  {
    this.GetMyApplications();
  }

  public GetMyApplications(): void
  {
    this.formsService.GetMyApplications().subscribe(x => this.Applications = x);
  }

}
