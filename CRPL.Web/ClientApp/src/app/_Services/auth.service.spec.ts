import {inject, TestBed} from '@angular/core/testing';

import {AuthService} from './auth.service';
import {HttpClientTestingModule, HttpTestingController} from "@angular/common/http/testing";
import {Router} from "@angular/router";
import {AccountStatus, UserAccountViewModel} from "../_Models/Account/UserAccountViewModel";

describe('AuthService', () =>
{
  let service: AuthService;
  let routerMock = jasmine.createSpyObj('router', ['navigate']);

  beforeEach(() =>
  {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        {provide: Router, useValue: routerMock},
        {provide: 'BASE_URL', useValue: ''}]
    });
    service = TestBed.inject(AuthService);
  });

  it('should be created', () =>
  {
    expect(service).toBeTruthy();
  });

  it('should fetch nonce', inject(
      [HttpTestingController, AuthService],
      (httpMock: HttpTestingController, authService: AuthService) =>
      {
        let mockNonce: string = "TEST NONCE";
        service['Address'] = 'TEST ADDRESS';

        authService.fetchNonce().subscribe((nonce: string) =>
        {
          expect(nonce).toEqual("TEST NONCE")
        });

        let request = httpMock.expectOne("user/nonce?walletAddress=TEST%20ADDRESS");

        expect(request.request.responseType).toEqual('json');
        expect(request.cancelled).toBeFalsy();

        request.flush(mockNonce);
      }
    )
  );

  it('should use exist user', inject(
    [HttpTestingController, AuthService],
    (httpMock: HttpTestingController, authService: AuthService) =>
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
      let mockToken = "TEST TOKEN";
      authService.UserAccount.next(mockUser);

      authService.Authenticate(mockToken).subscribe(user =>
      {
        expect(user).toEqual(mockUser);
      });

      httpMock.expectNone('user/auth?token=' + encodeURI(mockToken));
    }));

  it('should authenticate', inject(
    [HttpTestingController, AuthService],
    (httpMock: HttpTestingController, authService: AuthService) =>
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
      let mockToken = "TEST TOKEN";

      authService.Authenticate(mockToken).subscribe(user =>
      {
        expect(user).toEqual(mockUser);
      });

      let request = httpMock.expectOne('user/auth?token=' + encodeURI(mockToken));

      expect(request.request.responseType).toEqual('json');
      expect(request.cancelled).toBeFalsy();

      request.flush(mockUser);
    }));

  it('should logout', inject(
    [HttpTestingController, AuthService],
    (httpMock: HttpTestingController, authService: AuthService) =>
    {

      routerMock.navigate.and.returnValue(new Promise<boolean>(() => true));

      authService.Logout();

      expect(localStorage.getItem('authentication_token')).toBeFalsy();
      expect(localStorage.getItem('expires_at')).toBeFalsy();
      expect(authService.IsAuthenticated.getValue()).toBeFalsy();
      expect(authService.UserAccount.getValue()).toEqual(null as any);
      expect(authService['Address']).toEqual(undefined);
      expect(authService['AuthenticationToken']).toEqual(undefined);

      expect(routerMock.navigate).toHaveBeenCalledOnceWith(['/']);

    }));
});
