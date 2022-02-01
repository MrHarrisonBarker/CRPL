import {TestBed} from '@angular/core/testing';

import {WorksService} from './works.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {AuthService} from "./auth.service";
import {AlertService} from "./alert.service";
import {Router} from "@angular/router";

describe('WorksService', () =>
{
  let service: WorksService;

  beforeEach(() =>
  {
    let routerMock = jasmine.createSpyObj('router', ['navigate']);
    let alertMock = jasmine.createSpyObj('alertMock', ['Alert'])

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        {provide: AlertService, useValue: alertMock},
        {provide: Router, useValue: routerMock},
        {provide: 'BASE_URL', useValue: ''}]
    });
    service = TestBed.inject(WorksService);
  });

  it('should be created', () =>
  {
    expect(service).toBeTruthy();
  });
});
