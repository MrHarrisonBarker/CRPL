import { TestBed } from '@angular/core/testing';

import { RememberedGuard } from './remembered.guard';

describe('RememberedGuard', () => {
  let guard: RememberedGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(RememberedGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
