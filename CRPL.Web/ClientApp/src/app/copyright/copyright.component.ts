import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {CopyrightService} from "../_Services/copyright.service";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {ClarityIcons, undoIcon} from "@cds/core/icon";
import {Location} from "@angular/common";
import {AuthService} from "../_Services/auth.service";
import {AlertService} from "../_Services/alert.service";
import {Observable} from "rxjs";
import {WarehouseService} from "../_Services/warehouse.service";
import {map} from "rxjs/operators";

@Component({
  selector: 'app-copyright',
  templateUrl: './copyright.component.html',
  styleUrls: ['./copyright.component.css']
})
export class CopyrightComponent implements OnInit
{
  public CopyrightAsync!: Observable<RegisteredWorkViewModel>;
  public Copyright!: RegisteredWorkViewModel;

  public NoWork: boolean = false;
  public DisputeOpen: boolean = false;
  public Locked: boolean = false;
  public Error!: string;

  constructor (
    private route: ActivatedRoute,
    private copyrightService: CopyrightService,
    private location: Location,
    private authService: AuthService,
    private router: Router,
    private alertService: AlertService,
    private warehouse: WarehouseService)
  {
    ClarityIcons.addIcons(undoIcon);
  }

  async ngOnInit (): Promise<any>
  {
    let workId = this.route.snapshot.paramMap.get('id');
    if (workId)
    {
      this.copyrightService.Get(workId).subscribe();
      this.CopyrightAsync = this.warehouse.__MyWorks.pipe(map(x => x.find(w => w.Id == workId))) as Observable<RegisteredWorkViewModel>
    } else this.NoWork = true;

    this.CopyrightAsync.subscribe(x =>
    {
      this.Copyright = x;
    }, error => this.Error = error.error);
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

  public CancelDispute (): void
  {
  }
}
