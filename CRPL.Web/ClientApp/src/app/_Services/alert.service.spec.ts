import {fakeAsync, TestBed, tick} from '@angular/core/testing';

import {AlertMeta, AlertService} from './alert.service';
import {BehaviorSubject, Subject} from "rxjs";

describe('AlertService', () =>
{
  let service: AlertService;

  beforeEach(() =>
  {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AlertService);
  });

  it('should be created', () =>
  {
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

  it('should return alert subject', () =>
  {
    expect(service.alert).toEqual(new Subject<AlertMeta>())
  });

  it('should alert', () =>
  {
    let mockAlert: AlertMeta = {
      Message: 'MESSAGE',
      Type: 'danger'
    }
    service.Alert(mockAlert);
    service.alert.subscribe(a =>
    {
      expect(a).toEqual(mockAlert);
    });
  });
});
