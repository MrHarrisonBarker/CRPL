import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Observable, of} from 'rxjs';
import {AuthService} from "../_Services/auth.service";
import {AlertService} from "../_Services/alert.service";
import {map, switchMap} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class RememberedGuard implements CanActivate
{

  constructor (private authService: AuthService, private alertService: AlertService)
  {
  }

  canActivate (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean>
  {
    if (this.authService.AuthenticationToken != null && !this.authService.IsAuthenticated.getValue()) return this.authService.Authenticate(this.authService.AuthenticationToken).pipe(map(x => x != null));
    return of (true);
  }

}
