import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {CopyrightService} from "../_Services/copyright.service";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {ClarityIcons, undoIcon} from "@cds/core/icon";
import {Location} from "@angular/common";
import {AuthService} from "../_Services/auth.service";
import {AlertService} from "../_Services/alert.service";

@Component({
  selector: 'app-copyright',
  templateUrl: './copyright.component.html',
  styleUrls: ['./copyright.component.css']
})
export class CopyrightComponent implements OnInit
{
  public Copyright!: RegisteredWorkViewModel;

  constructor (
    private route: ActivatedRoute,
    private copyrightService: CopyrightService,
    private location: Location,
    private authService: AuthService,
    private router: Router,
    private alertService: AlertService)
  {
    ClarityIcons.addIcons(undoIcon);
  }

  async ngOnInit (): Promise<any>
  {
    this.alertService.StartLoading();
    let workId = this.route.snapshot.paramMap.get('id');
    if (workId) await this.copyrightService.Get(workId).subscribe(x =>
    {
      this.Copyright = x;
      this.alertService.StopLoading();
    });
  }

  public IsOwner (): boolean
  {
    if (!this.Copyright?.OwnershipStructure) return false;
    if (this.authService.UserAccount.getValue())
    {
      return this.Copyright.OwnershipStructure?.findIndex(x => x.Owner.toLowerCase() == this.authService.UserAccount.getValue().WalletPublicAddress.toLowerCase()) > -1;
    }
    return false;
  }

  public Back (): void
  {
    this.location.back();
  }

  public NavigateToDashboard (): void
  {
    this.router.navigate(['/dashboard', {workId: this.Copyright.Id}]);
  }
}
