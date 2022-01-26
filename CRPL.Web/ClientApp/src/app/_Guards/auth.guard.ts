import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from '@angular/router';
import {Observable, of} from 'rxjs';
import {AuthService} from "../_Services/auth.service";
import {map, switchMap} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate
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
        console.log("no token found so navigating away")
        this.router.navigate(['/']);
        return of(false);
      }

      return this.authService.Authenticate(this.authService.getToken()).pipe(map(usr => usr == null));
    }));
  }
}
