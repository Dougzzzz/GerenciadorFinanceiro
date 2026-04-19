import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ButtonComponent } from './button.component';
import { By } from '@angular/platform-browser';

describe('ButtonComponent', () => {
  let component: ButtonComponent;
  let fixture: ComponentFixture<ButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ButtonComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render correct variant class', () => {
    component.variant = 'outline';
    fixture.detectChanges();
    const btn = fixture.debugElement.query(By.css('button')).nativeElement;
    expect(btn.classList).toContain('btn-outline');
  });

  it('should render correct size class', () => {
    component.size = 'sm';
    fixture.detectChanges();
    const btn = fixture.debugElement.query(By.css('button')).nativeElement;
    expect(btn.classList).toContain('btn-sm');
  });

  it('should emit clicked event on click', () => {
    spyOn(component.clicked, 'emit');
    const btn = fixture.debugElement.query(By.css('button'));
    btn.nativeElement.click();
    expect(component.clicked.emit).toHaveBeenCalled();
  });

  it('should be disabled when input is set', () => {
    component.disabled = true;
    fixture.detectChanges();
    const btn = fixture.debugElement.query(By.css('button')).nativeElement;
    expect(btn.disabled).toBeTrue();

    spyOn(component.clicked, 'emit');
    btn.click();
    expect(component.clicked.emit).not.toHaveBeenCalled();
  });
});
