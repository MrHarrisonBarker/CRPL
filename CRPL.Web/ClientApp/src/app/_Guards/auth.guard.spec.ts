import {inject, TestBed} from '@angular/core/testing';

import {AuthGuard} from './auth.guard';
import {HttpClientTestingModule, HttpTestingController} from "@angular/common/http/testing";
import {AuthService} from "../_Services/auth.service";
import {AlertService} from "../_Services/alert.service";
import {ActivatedRouteSnapshot, Router, RouterStateSnapshot} from "@angular/router";
import {setProp} from "../test.utils";
import {BehaviorSubject, of, throwError} from "rxjs";
import {UserService} from "../_Services/user.service";
import {HttpErrorResponse} from "@angular/common/http";
import {AccountStatus, UserAccountViewModel} from "../_Models/Account/UserAccountViewModel";
import {tap} from "rxjs/operators";

describe('AuthGuard', () =>
{
  let guard: AuthGuard;

  let routerMock = jasmine.createSpyObj('router', ['navigate']);
  let alertMock = jasmine.createSpyObj('alertMock', ['Alert']);
  let authMock = jasmine.createSpyObj('AuthService', ['getToken', 'Authenticate'], ['IsAuthenticated'])

  beforeEach(() =>
  {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthGuard,
        {provide: AuthService, useValue: authMock},
        {provide: AlertService, useValue: alertMock},
        {provide: Router, useValue: routerMock},
        {provide: 'BASE_URL', useValue: ''}]
    });
    guard = TestBed.inject(AuthGuard);
  });

  it('should be created', () =>
  {
    expect(guard).toBeTruthy();
  });

  it('should return true if already authenticated', () =>
  {
    setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(true));

    guard.canActivate({} as ActivatedRouteSnapshot, <RouterStateSnapshot>{url: 'testUrl'}).subscribe(res =>
    {
      expect(res).toBeTruthy();
    });
  });

  it('should route away if no token', () =>
  {
    setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(false));
    authMock.getToken.and.returnValue(null);
    routerMock.navigate.and.returnValue(new Promise<boolean>(() => true));

    guard.canActivate({} as ActivatedRouteSnapshot, <RouterStateSnapshot>{url: 'testUrl'}).subscribe(res =>
    {
      expect(routerMock.navigate).toHaveBeenCalledOnceWith(['/']);
      expect(res).toBeFalsy();
    });

  });

  it('should route away if error thrown', () =>
  {
    authMock.Authenticate.and.returnValue(throwError(new Error("This is an error")));
    setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(false));
    authMock.getToken.and.returnValue("TEST TOKEN");
    routerMock.navigate.and.returnValue(new Promise<boolean>(() => true));

    guard.canActivate({} as ActivatedRouteSnapshot, <RouterStateSnapshot>{url: 'testUrl'}).subscribe(res =>
    {
      console.log(res);
      expect(alertMock.Alert).toHaveBeenCalledTimes(1);
      expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
      expect(res).toBeFalsy();
    });
  });

  it('should return true if authenticated', () =>
  {
    let mockUser: UserAccountViewModel = {
      DateOfBirth: {Day: 0, Month: 0, Year: 0},
      DialCode: "",
      Email: "",
      FirstName: "",
      Id: "",
      LastName: "",
      PhoneNumber: "",
      RegisteredJurisdiction: "",
      Status: AccountStatus.Incomplete,
      WalletPublicAddress: ""
    }
    authMock.Authenticate.and.returnValue(of(mockUser));
    setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(false));
    authMock.getToken.and.returnValue("TEST TOKEN");
    routerMock.navigate.and.returnValue(new Promise<boolean>(() => true));

    guard.canActivate({} as ActivatedRouteSnapshot, <RouterStateSnapshot>{url: 'testUrl'}).subscribe(res =>
    {
      console.log(res);
      expect(res).toBeTruthy();
    });
  });

});
