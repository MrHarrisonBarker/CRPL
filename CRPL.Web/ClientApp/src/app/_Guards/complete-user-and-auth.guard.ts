import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from '@angular/router';
import {Observable, of} from 'rxjs';
import {AuthService} from "../_Services/auth.service";
import {catchError, map, switchMap} from "rxjs/operators";
import {AccountStatus} from "../_Models/Account/UserAccountViewModel";

@Injectable({
  providedIn: 'root'
})
export class CompleteUserAndAuthGuard implements CanActivate
{

  constructor (private authService: AuthService, private router: Router)
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

      if (!this.authService.getToken())
      {
        console.log("no token found so navigating away");
        // TODO: show error message
        this.router.navigate(['/']);
        return of(false);
      }

      return this.authService.Authenticate(this.authService.getToken()).pipe(map(authenticatedUser =>
      {
        console.log("got user now checking status", authenticatedUser);
        if (authenticatedUser == null) return false;
        if (authenticatedUser.Status == AccountStatus.Complete) return true;

        console.log("the user has not complete their profile navigating to wizard");
        this.router.navigate(['/user/info']);

        return false;
      })).pipe(catchError(err =>
      {
        // TODO: show error message
        this.router.navigate(['/']);
        return of(false);
      }));
    }));
  }

}
