import { TestBed } from '@angular/core/testing';

import { AuthGuard } from './auth.guard';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {AuthService} from "../_Services/auth.service";
import {AlertService} from "../_Services/alert.service";
import {Router} from "@angular/router";

describe('AuthGuard', () => {
  let guard: AuthGuard;

  beforeEach(() => {
    let routerMock = jasmine.createSpyObj('router', ['navigate']);
    let alertMock = jasmine.createSpyObj('alertMock', ['Alert']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        {provide: AlertService, useValue: alertMock},
        {provide: Router, useValue: routerMock},
        {provide: 'BASE_URL', useValue: ''}]
    });
    guard = TestBed.inject(AuthGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
