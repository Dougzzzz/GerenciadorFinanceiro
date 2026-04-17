import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CardComponent } from './card.component';
import { By } from '@angular/platform-browser';

describe('CardComponent', () => {
  let component: CardComponent;
  let fixture: ComponentFixture<CardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(CardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show header only when title is provided', () => {
    let header = fixture.debugElement.query(By.css('.card-header'));
    expect(header).toBeNull();

    component.title = 'Test Title';
    fixture.detectChanges();

    header = fixture.debugElement.query(By.css('.card-header'));
    expect(header).not.toBeNull();
    expect(header.nativeElement.textContent).toContain('Test Title');
  });
});
