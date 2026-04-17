import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DashboardSpendingChartComponent } from './dashboard-spending-chart.component';

describe('DashboardSpendingChartComponent', () => {
  let fixture: ComponentFixture<DashboardSpendingChartComponent>;
  let component: DashboardSpendingChartComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardSpendingChartComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardSpendingChartComponent);
    component = fixture.componentInstance;
  });

  it('deve exibir estado vazio quando nao houver metas', () => {
    component.metasResumo = [];

    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Nenhuma meta de gasto definida para este mês.');
  });

  it('deve limitar a largura da barra de gasto a 100%', () => {
    expect(
      component.calculateWidth({
        categoria: 'Lazer',
        meta: 100,
        gastoAtual: 180,
        percentual: 1.8,
      }),
    ).toBe(100);
  });

  it('deve retornar 0 quando a meta for zero', () => {
    expect(
      component.calculateWidth({
        categoria: 'Lazer',
        meta: 0,
        gastoAtual: 50,
        percentual: 0.5,
      }),
    ).toBe(0);
  });

  it('deve aplicar destaque visual quando o gasto exceder a meta', () => {
    component.metasResumo = [
      {
        categoria: 'Mercado',
        meta: 100,
        gastoAtual: 150,
        percentual: 1.5,
      },
    ];

    fixture.detectChanges();

    const percent = fixture.nativeElement.querySelector('.usage-percent') as HTMLElement;
    const actualBar = fixture.nativeElement.querySelector('.actual-bar') as HTMLElement;

    expect(percent.classList.contains('exceeded')).toBeTrue();
    expect(actualBar.classList.contains('exceeded')).toBeTrue();
    expect(actualBar.style.width).toBe('100%');
  });

  it('deve calcular largura zero se meta for zero', () => {
    const width = component.calculateWidth({ meta: 0, gastoAtual: 100 } as any);
    expect(width).toBe(0);
  });

  it('deve mostrar percentual excedido na classe CSS', () => {
    component.metasResumo = [{ categoria: 'C', meta: 100, gastoAtual: 200, percentual: 2 }];
    fixture.detectChanges();
    const percent = fixture.nativeElement.querySelector('.usage-percent');
    expect(percent.classList).toContain('exceeded');
  });
});
