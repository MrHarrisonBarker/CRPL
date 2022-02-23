import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {CopyrightService} from "../_Services/copyright.service";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {ClarityIcons, undoIcon} from "@cds/core/icon";
import {Location} from "@angular/common";
import {AuthService} from "../_Services/auth.service";
import {AlertService} from "../_Services/alert.service";
import {OwnershipRestructureViewModel} from "../_Models/Applications/OwnershipRestructureViewModel";
import {ApplicationType} from "../_Models/Applications/ApplicationViewModel";
import {ApplicationStatus} from "../_Models/Applications/ApplicationStatus";
import {DisputeViewModel} from "../_Models/Applications/DisputeViewModel";

@Component({
  selector: 'app-copyright',
  templateUrl: './copyright.component.html',
  styleUrls: ['./copyright.component.css']
})
export class CopyrightComponent implements OnInit
{
  public Copyright!: RegisteredWorkViewModel;
  public Loaded: boolean = false;
  public DisputeOpen: boolean = false;

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

    let workId = this.route.snapshot.paramMap.get('id');
    if (workId) await this.copyrightService.Get(workId).subscribe(x =>
    {
      this.Copyright = x;
      this.Loaded = true;
    }, error => this.Loaded = true, () => this.Loaded = true);
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

  get ExistingDispute (): DisputeViewModel
  {
    if (this.Copyright.AssociatedApplication)
    {
      return this.Copyright.AssociatedApplication.find(x => x.ApplicationType == ApplicationType.OwnershipRestructure && x.Status == ApplicationStatus.Incomplete) as OwnershipRestructureViewModel;
    }
    return undefined as any;
  }

  public CancelDispute (): void
  {

  }
}
