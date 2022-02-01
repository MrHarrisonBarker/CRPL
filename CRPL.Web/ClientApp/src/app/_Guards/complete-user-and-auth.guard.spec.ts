import { TestBed } from '@angular/core/testing';

import { CompleteUserAndAuthGuard } from './complete-user-and-auth.guard';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {AuthService} from "../_Services/auth.service";
import {AlertService} from "../_Services/alert.service";
import {Router} from "@angular/router";

describe('CompleteUserAndAuthGuard', () => {
  let guard: CompleteUserAndAuthGuard;

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
    guard = TestBed.inject(CompleteUserAndAuthGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
