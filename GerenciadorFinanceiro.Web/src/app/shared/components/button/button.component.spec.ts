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

  it('should apply variant and size classes', () => {
    component.variant = 'outline';
    component.size = 'sm';
    fixture.detectChanges();

    const button = fixture.debugElement.query(By.css('button')).nativeElement;
    expect(button.classList).toContain('btn-outline');
    expect(button.classList).toContain('btn-sm');
  });

  it('should be disabled when input is true', () => {
    component.disabled = true;
    fixture.detectChanges();

    const button = fixture.debugElement.query(By.css('button')).nativeElement;
    expect(button.disabled).toBeTrue();
  });

  it('should emit onClick event when clicked', () => {
    spyOn(component.onClick, 'emit');
    const button = fixture.debugElement.query(By.css('button'));
    
    button.nativeElement.click();
    
    expect(component.onClick.emit).toHaveBeenCalled();
  });

  it('should not emit onClick event when disabled', () => {
    spyOn(component.onClick, 'emit');
    component.disabled = true;
    fixture.detectChanges();
    
    const button = fixture.debugElement.query(By.css('button'));
    button.nativeElement.click();
    
    expect(component.onClick.emit).not.toHaveBeenCalled();
  });
});
