import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TransacoesListComponent } from './transacoes-list.component';
import { TipoTransacao } from '../../core/models/financeiro.model';

describe('TransacoesListComponent', () => {
  let fixture: ComponentFixture<TransacoesListComponent>;
  let component: TransacoesListComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransacoesListComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TransacoesListComponent);
    component = fixture.componentInstance;
    component.transacoes = [
      {
        id: 'transacao-1',
        data: '2026-04-10T00:00:00Z',
        descricao: 'Mercado',
        valor: -150,
        tipo: TipoTransacao.Despesa,
        categoriaId: 'cat-1',
        categoria: 'Alimentacao',
        nomeCartao: '',
        finalCartao: '',
        parcela: '',
        cotacao: 1,
      },
    ];
    fixture.detectChanges();
  });

  it('deve emitir ordenacao ascendente ao clicar em uma nova coluna', () => {
    spyOn(component.onSort, 'emit');

    component.ordenarPor = 'Data';
    component.direcao = 'Desc';

    component.toggleSort('Valor');

    expect(component.onSort.emit).toHaveBeenCalledWith({ coluna: 'Valor', direcao: 'Asc' });
  });

  it('deve inverter a direcao ao clicar novamente na mesma coluna', () => {
    spyOn(component.onSort, 'emit');

    component.ordenarPor = 'Valor';
    component.direcao = 'Asc';

    component.toggleSort('Valor');

    expect(component.onSort.emit).toHaveBeenCalledWith({ coluna: 'Valor', direcao: 'Desc' });
  });

  it('deve retornar o icone correto para a coluna ordenada', () => {
    component.ordenarPor = 'Valor';
    component.direcao = 'Asc';

    expect(component.getSortIcon('Valor')).toBe('🔼');
    expect(component.getSortIcon('Data')).toBe('↕️');
  });

  it('deve renderizar a descricao e a categoria da transacao', () => {
    const text = fixture.nativeElement.textContent as string;

    expect(text).toContain('Mercado');
    expect(text).toContain('Alimentacao');
  });
});
