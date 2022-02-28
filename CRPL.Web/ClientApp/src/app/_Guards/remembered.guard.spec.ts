import { TestBed } from '@angular/core/testing';

import { RememberedGuard } from './remembered.guard';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {AuthService} from "../_Services/auth.service";
import {AlertService} from "../_Services/alert.service";
import {Router} from "@angular/router";

describe('RememberedGuard', () => {

  let guard: RememberedGuard;
  let routerMock = jasmine.createSpyObj('router', ['navigate']);
  let alertMock = jasmine.createSpyObj('alertMock', ['Alert']);
  let authMock = jasmine.createSpyObj('AuthService', ['getToken', 'Authenticate', 'Logout'], ['IsAuthenticated']);

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        RememberedGuard,
        {provide: AuthService, useValue: authMock},
        {provide: AlertService, useValue: alertMock},
        {provide: Router, useValue: routerMock},
        {provide: 'BASE_URL', useValue: ''}]
    });
    guard = TestBed.inject(RememberedGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
