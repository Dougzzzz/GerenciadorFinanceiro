import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProgressBarComponent } from './progress-bar.component';

describe('ProgressBarComponent', () => {
  let component: ProgressBarComponent;
  let fixture: ComponentFixture<ProgressBarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProgressBarComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ProgressBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should apply width and color style', () => {
    component.value = 75;
    component.color = 'red';
    fixture.detectChanges();

    const fill = fixture.nativeElement.querySelector('.progress-fill') as HTMLElement;
    expect(fill.style.width).toBe('75%');
    expect(fill.style.backgroundColor).toBe('red');
  });
});
