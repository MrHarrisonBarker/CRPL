import {inject, TestBed} from '@angular/core/testing';

import {UserService} from './user.service';
import {HttpClientTestingModule, HttpTestingController} from "@angular/common/http/testing";
import {AuthService} from "./auth.service";
import {Router} from "@angular/router";
import {AlertService} from "./alert.service";
import {AccountInputModel} from "../_Models/Account/AccountInputModel";
import {UserAccountStatusModel} from "../_Models/Account/UserAccountStatusModel";
import {BehaviorSubject} from "rxjs";
import {AccountStatus, UserAccountViewModel} from "../_Models/Account/UserAccountViewModel";
import {setProp} from "../test.utils";

describe('UserService', () =>
{
  let service: UserService;
  let routerMock = jasmine.createSpyObj('router', ['navigate']);
  let alertMock = jasmine.createSpyObj('alertMock', ['Alert']);
  let authMock = jasmine.createSpyObj('AuthService', [''], ['IsAuthenticated', 'UserAccount'])

  beforeEach(() =>
  {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        UserService,
        {provide: AuthService, useValue: authMock},
        {provide: AlertService, useValue: alertMock},
        {provide: Router, useValue: routerMock},
        {provide: 'BASE_URL', useValue: ''}]
    });
    service = TestBed.inject(UserService);

    setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(true));
  });

  it('should be created', () =>
  {
    expect(service).toBeTruthy();
  });

  it('should update account', inject(
    [HttpTestingController, UserService],
    (httpMock: HttpTestingController, userService: UserService) =>
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

      setProp<BehaviorSubject<UserAccountViewModel>>(authMock, 'UserAccount', new BehaviorSubject<UserAccountViewModel>(mockUser));

      let mockResponse: UserAccountStatusModel = {
        PartialFields: [],
        UserAccount: mockUser
      };

      let updateInput: AccountInputModel = {};

      userService.UpdateAccount(updateInput).subscribe(statusModel =>
      {
        expect(statusModel).toEqual(mockResponse);
        expect(authMock.UserAccount.getValue()).toEqual(mockUser);
      });

      let request = httpMock.expectOne("user?accountId=" + mockUser.Id);

      expect(request.request.responseType).toEqual('json');
      expect(request.cancelled).toBeFalsy();

      request.flush(mockResponse);
    }));

  it('should throw if not authenticated', inject(
    [HttpTestingController, UserService],
    (httpMock: HttpTestingController, userService: UserService) =>
    {
      setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(false));

      userService.UpdateAccount({}).subscribe(() =>
      {
      }, error => expect(error).toEqual(new Error("The user is not authenticated!")))
    }));

  it('should find if phone exits', inject(
    [HttpTestingController, UserService],
    (httpMock: HttpTestingController, userService: UserService) =>
    {
      let mockPhone = "PHONE NUMBER";
      let mockResponse = true;

      userService.PhoneExists(mockPhone).subscribe(answer =>
      {
        expect(answer).toBeTruthy();
      });

      let request = httpMock.expectOne("user/unique/phone?phone=" + encodeURI(mockPhone));

      expect(request.request.responseType).toEqual('json');
      expect(request.cancelled).toBeFalsy();

      request.flush(mockResponse);
    }));

  it('should find if email exits', inject(
    [HttpTestingController, UserService],
    (httpMock: HttpTestingController, userService: UserService) =>
    {
      let mockEmail = "EMAIL";
      let mockResponse = true;

      userService.EmailExists(mockEmail).subscribe(answer =>
      {
        expect(answer).toBeTruthy();
      });

      let request = httpMock.expectOne("user/unique/email?email=" + encodeURI(mockEmail));

      expect(request.request.responseType).toEqual('json');
      expect(request.cancelled).toBeFalsy();

      request.flush(mockResponse);
    }));
});
