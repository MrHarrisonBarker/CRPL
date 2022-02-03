import {TestBed} from '@angular/core/testing';

import {CompleteUserAndAuthGuard} from './complete-user-and-auth.guard';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {AuthService} from "../_Services/auth.service";
import {AlertService} from "../_Services/alert.service";
import {ActivatedRouteSnapshot, Router, RouterStateSnapshot} from "@angular/router";
import {AuthGuard} from "./auth.guard";
import {setProp} from "../test.utils";
import {BehaviorSubject, of} from "rxjs";
import {AccountStatus, UserAccountViewModel} from "../_Models/Account/UserAccountViewModel";

describe('CompleteUserAndAuthGuard', () =>
{
  let guard: CompleteUserAndAuthGuard;

  let routerMock = jasmine.createSpyObj('router', ['navigate']);
  let alertMock = jasmine.createSpyObj('alertMock', ['Alert']);
  let authMock = jasmine.createSpyObj('AuthService', ['getToken', 'Authenticate', 'Logout'], ['IsAuthenticated']);

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
    guard = TestBed.inject(CompleteUserAndAuthGuard);
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
      expect(routerMock.navigate).toHaveBeenCalled();
      expect(res).toBeFalsy();
    });

  });

  it('should return false and route away if no user', () =>
  {
    setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(false));
    authMock.getToken.and.returnValue("TEST TOKEN");
    routerMock.navigate.and.returnValue(new Promise<boolean>(() => true));
    authMock.Authenticate.and.returnValue(of(null));

    guard.canActivate({} as ActivatedRouteSnapshot, <RouterStateSnapshot>{url: 'testUrl'}).subscribe(res =>
    {
      expect(routerMock.navigate).toHaveBeenCalled();
      expect(res).toBeFalsy();
    });
  });

  it('should route to info wizard if user not complete', () =>
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
    };

    setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(false));
    authMock.getToken.and.returnValue("TEST TOKEN");
    routerMock.navigate.and.returnValue(new Promise<boolean>(() => true));
    authMock.Authenticate.and.returnValue(of(mockUser));

    guard.canActivate({} as ActivatedRouteSnapshot, <RouterStateSnapshot>{url: 'testUrl'}).subscribe(res =>
    {
      expect(routerMock.navigate).toHaveBeenCalledWith(['/user/info']);
      expect(res).toBeFalsy();
    });
  });


});
