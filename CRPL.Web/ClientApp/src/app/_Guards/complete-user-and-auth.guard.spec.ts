import { TestBed } from '@angular/core/testing';

import { CompleteUserAndAuthGuard } from './complete-user-and-auth.guard';

describe('CompleteUserAndAuthGuard', () => {
  let guard: CompleteUserAndAuthGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(CompleteUserAndAuthGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
