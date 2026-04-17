import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ContasListComponent } from './contas-list.component';

describe('ContasListComponent', () => {
  let component: ContasListComponent;
  let fixture: ComponentFixture<ContasListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContasListComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ContasListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render account list', () => {
    component.contas = [{ id: '1', nomeBanco: 'Nubank', saldoAtual: 500, provedor: 2 } as any];
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.item')).toBeTruthy();
    expect(compiled.querySelector('span')?.textContent).toContain('Nubank');
  });
});
