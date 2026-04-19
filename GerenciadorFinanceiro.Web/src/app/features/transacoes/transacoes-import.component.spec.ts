import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { TransacoesImportComponent } from './transacoes-import.component';
import { TipoTransacao, ProvedorExtrato } from '../../core/models/financeiro.model';

describe('TransacoesImportComponent', () => {
  let fixture: ComponentFixture<TransacoesImportComponent>;
  let component: TransacoesImportComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransacoesImportComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TransacoesImportComponent);
    component = fixture.componentInstance;
    component.categorias = [{ id: 'cat-1', nome: 'Outros', tipo: TipoTransacao.Despesa }];
    component.contas = [{ id: 'conta-1', nomeBanco: 'Inter', saldoAtual: 1000, provedor: ProvedorExtrato.Generico }];
    component.cartoes = [{ id: 'cartao-1', nome: 'Nubank', limite: 5000, diaFechamento: 10, diaVencimento: 20, provedor: ProvedorExtrato.Nubank }];
    fixture.detectChanges();
  });

  it('deve manter o botao de processar desabilitado sem arquivo e sem conta/cartao', () => {
    const buttons = fixture.debugElement.queryAll(By.directive(ButtonComponent));
    const processarButton = buttons[1].componentInstance as ButtonComponent;

    expect(processarButton.disabled).toBeTrue();
  });

  it('deve habilitar o botao de processar quando houver arquivo e conta selecionada', () => {
    component.selectedFile = new File(['conteudo'], 'extrato.csv', { type: 'text/csv' });
    component.destino = 'conta';
    component.config.contaId = 'conta-1';

    fixture.detectChanges();

    const buttons = fixture.debugElement.queryAll(By.directive(ButtonComponent));
    const processarButton = buttons[1].componentInstance as ButtonComponent;

    expect(processarButton.disabled).toBeFalse();
  });

  it('deve capturar o arquivo selecionado no input', () => {
    const file = new File(['conteudo'], 'extrato.csv', { type: 'text/csv' });

    component.onFileChange({
      target: {
        files: [file],
      },
    } as unknown as Event);

    expect(component.selectedFile).toBe(file);
  });

  it('deve emitir o evento de importacao com arquivo e configuracao', () => {
    const file = new File(['conteudo'], 'extrato.csv', { type: 'text/csv' });
    spyOn(component.imported, 'emit');

    component.selectedFile = file;
    component.config = {
      categoriaId: 'cat-1',
      contaId: 'conta-1',
      cartaoId: undefined,
    };

    component.confirmarImportacao();

    expect(component.imported.emit).toHaveBeenCalledWith({
      file,
      config: {
        categoriaId: 'cat-1',
        contaId: 'conta-1',
        cartaoId: undefined,
      },
    });
  });

  it('nao deve emitir importacao quando nenhum arquivo estiver selecionado', () => {
    spyOn(component.imported, 'emit');
    component.config.contaId = 'conta-1';

    component.confirmarImportacao();

    expect(component.imported.emit).not.toHaveBeenCalled();
  });

  it('deve habilitar o botao de verificar quando houver arquivo e destino selecionados', () => {
    component.selectedFile = new File(['conteudo'], 'extrato.csv', { type: 'text/csv' });
    component.destino = 'conta';
    component.config.contaId = 'conta-1';
    
    fixture.detectChanges();
    expect(component.canProcess()).toBeTrue();
  });
});
