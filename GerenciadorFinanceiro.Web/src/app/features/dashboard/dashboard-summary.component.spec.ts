import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DashboardSummaryComponent } from './dashboard-summary.component';

describe('DashboardSummaryComponent', () => {
  let fixture: ComponentFixture<DashboardSummaryComponent>;
  let component: DashboardSummaryComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardSummaryComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardSummaryComponent);
    component = fixture.componentInstance;
  });

  it('deve renderizar os totais formatados em BRL', () => {
    component.totalReceitas = 5000;
    component.totalDespesas = 1250.5;
    component.saldo = 3749.5;

    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;

    expect(text).toContain('Total Receitas');
    expect(text).toContain('R$');
    expect(text).toContain('5,000.00');
    expect(text).toContain('1,250.50');
    expect(text).toContain('3,749.50');
  });

  it('deve aplicar a classe negative quando o saldo for menor que zero', () => {
    component.saldo = -20;

    fixture.detectChanges();

    const saldo = fixture.nativeElement.querySelector('.card-balance .value') as HTMLElement;

    expect(saldo.classList.contains('negative')).toBeTrue();
  });

  it('nao deve aplicar a classe negative quando o saldo for zero ou positivo', () => {
    component.saldo = 120;

    fixture.detectChanges();

    const saldo = fixture.nativeElement.querySelector('.card-balance .value') as HTMLElement;

    expect(saldo.classList.contains('negative')).toBeFalse();
  });
});
