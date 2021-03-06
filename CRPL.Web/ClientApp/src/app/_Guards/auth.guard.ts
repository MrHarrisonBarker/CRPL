import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, CanActivateChild, Router, RouterStateSnapshot} from '@angular/router';
import {Observable, of} from 'rxjs';
import {AuthService} from "../_Services/auth.service";
import {catchError, map, switchMap} from "rxjs/operators";
import {AlertService} from "../_Services/alert.service";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild
{

  constructor (
    private authService: AuthService,
    private router: Router,
    private alertService: AlertService)
  {
  }

  canActivate (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean>
  {
    return this.authService.IsAuthenticated.pipe(switchMap(isAuthenticated =>
    {
      if (isAuthenticated)
      {
        return of(true);
      }

      let token = this.authService.getToken();

      if (!token)
      {
        console.log("no token found so navigating away")
        this.router.navigate(['/']).then(r => null);
        return of(false);
      }

      return this.authService.Authenticate(token).pipe(map(usr => {
        console.log(usr);
        return usr != null;
      })).pipe(catchError(err =>
      {
        this.alertService.Alert({Message: "There was a problem when authenticating your account", Type: "danger"});
        this.router.navigate(['/']).then(r => null);
        return of(false);
      }));
    }));
  }

  canActivateChild (childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean>
  {
    return this.canActivate(childRoute, state);
  }
}
