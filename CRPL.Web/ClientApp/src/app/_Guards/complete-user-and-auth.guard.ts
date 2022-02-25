import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from '@angular/router';
import {Observable, of} from 'rxjs';
import {AuthService} from "../_Services/auth.service";
import {catchError, map, switchMap} from "rxjs/operators";
import {AccountStatus} from "../_Models/Account/UserAccountViewModel";
import {AlertService} from "../_Services/alert.service";

@Injectable({
  providedIn: 'root'
})
export class CompleteUserAndAuthGuard implements CanActivate
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
        console.log("no token found so navigating away");
        this.alertService.Alert({Message: "Session expired log back in", Type: "danger"});
        this.router.navigate(['/']).then(r => null);
        return of(false);
      }

      return this.authService.Authenticate(token).pipe(map(authenticatedUser =>
      {
        console.log("got user now checking status", authenticatedUser);
        if (authenticatedUser == null)
        {
          this.router.navigate(['/']).then(r => null);
          return false;
        }
        if (authenticatedUser.Status == AccountStatus.Complete) return true;

        console.log("the user has not complete their profile navigating to wizard");

        this.alertService.Alert({Message: "You have not completed the signup process yet, complete to gain access", Type: "information"});
        this.router.navigate(['/u/info']).then(r => null);

        return false;
      })).pipe(catchError(err =>
      {
        this.alertService.Alert({Message: "There was a problem when authenticating your account", Type: "danger"});

        // logout if authentication fails
        this.authService.Logout();
        this.router.navigate(['/']).then(r => null);
        return of(false);
      }));
    }));
  }

}
