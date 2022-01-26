import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoWizardComponent } from './info-wizard.component';

describe('InfoWizardComponent', () => {
  let component: InfoWizardComponent;
  let fixture: ComponentFixture<InfoWizardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InfoWizardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InfoWizardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
