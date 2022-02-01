import { TestBed } from '@angular/core/testing';

import { AlertService } from './alert.service';

describe('AlertService', () => {
  let service: AlertService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AlertService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should set loading false on construct', () =>
  {
    expect(service.loading.getValue()).toBeFalsy();
  });

  it('should start loading', () =>
  {
    service.StartLoading();
    expect(service.loading.getValue()).toBeTruthy();
  });

  it('should stop loading', () =>
  {
    service.StartLoading();
    service.StopLoading();
    expect(service.loading.getValue()).toBeFalsy();
  });
});
