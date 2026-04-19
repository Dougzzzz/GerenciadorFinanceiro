import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { TransacoesFormComponent } from './transacoes-form.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { TipoTransacao, ProvedorExtrato } from '../../core/models/financeiro.model';

describe('TransacoesFormComponent', () => {
  let fixture: ComponentFixture<TransacoesFormComponent>;
  let component: TransacoesFormComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransacoesFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TransacoesFormComponent);
    component = fixture.componentInstance;
    component.categorias = [
      { id: 'cat-1', nome: 'Alimentacao', tipo: TipoTransacao.Despesa },
      { id: 'cat-2', nome: 'Salario', tipo: TipoTransacao.Receita },
    ];
    component.contas = [{ id: 'conta-1', nomeBanco: 'Inter', saldoAtual: 1000, provedor: ProvedorExtrato.Generico }];
    component.cartoes = [{ id: 'cartao-1', nome: 'Nubank', limite: 5000, diaFechamento: 10, diaVencimento: 20, provedor: ProvedorExtrato.Nubank }];
    fixture.detectChanges();
  });

  it('deve desabilitar o botao de salvar enquanto os campos obrigatorios nao estiverem preenchidos', () => {
    const button = fixture.debugElement.query(By.directive(ButtonComponent)).componentInstance as ButtonComponent;

    expect(button.disabled).toBeTrue();
  });

  it('deve habilitar o botao de salvar quando descricao, valor e categoria forem informados', () => {
    component.dados.descricao = 'Supermercado';
    component.dados.valor = -120.5;
    component.dados.categoriaId = 'cat-1';

    fixture.detectChanges();

    const button = fixture.debugElement.query(By.directive(ButtonComponent)).componentInstance as ButtonComponent;

    expect(button.disabled).toBeFalse();
  });

  it('deve emitir os dados preenchidos e resetar o formulario ao salvar', () => {
    spyOn(component.saved, 'emit');

    component.dados = {
      data: '2026-04-17',
      descricao: 'Mercado',
      valor: -90,
      categoriaId: 'cat-1',
      contaBancariaId: 'conta-1',
      cartaoCreditoId: undefined,
    };

    component.salvar();

    expect(component.saved.emit).toHaveBeenCalledWith({
      data: '2026-04-17',
      descricao: 'Mercado',
      valor: -90,
      categoriaId: 'cat-1',
      contaBancariaId: 'conta-1',
      cartaoCreditoId: undefined,
    });
    expect(component.dados.descricao).toBe('');
    expect(component.dados.valor).toBe(0);
    expect(component.dados.categoriaId).toBe('');
    expect(component.dados.contaBancariaId).toBeUndefined();
    expect(component.dados.cartaoCreditoId).toBeUndefined();
    expect(component.dados.data).toMatch(/^\d{4}-\d{2}-\d{2}$/);
  });
});
