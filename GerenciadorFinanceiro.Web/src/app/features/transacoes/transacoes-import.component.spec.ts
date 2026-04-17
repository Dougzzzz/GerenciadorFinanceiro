import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { TransacoesImportComponent } from './transacoes-import.component';

describe('TransacoesImportComponent', () => {
  let fixture: ComponentFixture<TransacoesImportComponent>;
  let component: TransacoesImportComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransacoesImportComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TransacoesImportComponent);
    component = fixture.componentInstance;
    component.categorias = [{ id: 'cat-1', nome: 'Outros', tipo: 1 }];
    component.contas = [{ id: 'conta-1', nomeBanco: 'Inter', saldoAtual: 1000, provedor: 0 }];
    component.cartoes = [{ id: 'cartao-1', nome: 'Nubank', limite: 5000, diaFechamento: 10, diaVencimento: 20, provedor: 2 }];
    fixture.detectChanges();
  });

  it('deve manter o botao de processar desabilitado sem arquivo e sem conta/cartao', () => {
    const buttons = fixture.debugElement.queryAll(By.directive(ButtonComponent));
    const processarButton = buttons[1].componentInstance as ButtonComponent;

    expect(processarButton.disabled).toBeTrue();
  });

  it('deve habilitar o botao de processar quando houver arquivo e conta selecionada', () => {
    component.selectedFile = new File(['conteudo'], 'extrato.csv', { type: 'text/csv' });
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
    });

    expect(component.selectedFile).toBe(file);
  });

  it('deve emitir o evento de importacao com arquivo e configuracao', () => {
    const file = new File(['conteudo'], 'extrato.csv', { type: 'text/csv' });
    spyOn(component.onImport, 'emit');

    component.selectedFile = file;
    component.config = {
      categoriaId: 'cat-1',
      contaId: 'conta-1',
      cartaoId: undefined,
    };

    component.importar();

    expect(component.onImport.emit).toHaveBeenCalledWith({
      file,
      config: {
        categoriaId: 'cat-1',
        contaId: 'conta-1',
        cartaoId: undefined,
      },
    });
  });

  it('nao deve emitir importacao quando nenhum arquivo estiver selecionado', () => {
    spyOn(component.onImport, 'emit');
    component.config.contaId = 'conta-1';

    component.importar();

    expect(component.onImport.emit).not.toHaveBeenCalled();
  });
});
