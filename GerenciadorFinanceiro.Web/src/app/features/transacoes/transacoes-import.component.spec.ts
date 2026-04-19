import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { TransacoesImportComponent } from './transacoes-import.component';
import { TipoTransacao, ProvedorExtrato } from '../../core/models/financeiro.model';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { of } from 'rxjs';

describe('TransacoesImportComponent', () => {
  let fixture: ComponentFixture<TransacoesImportComponent>;
  let component: TransacoesImportComponent;
  let financeiroService: jasmine.SpyObj<FinanceiroService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('FinanceiroService', ['getPreviewImportacao', 'confirmarImportacao']);

    await TestBed.configureTestingModule({
      imports: [TransacoesImportComponent],
      providers: [
        { provide: FinanceiroService, useValue: spy }
      ]
    }).compileComponents();

    financeiroService = TestBed.inject(FinanceiroService) as jasmine.SpyObj<FinanceiroService>;
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

  it('deve chamar o servico de preview ao clicar em verificar', () => {
    const file = new File(['conteudo'], 'extrato.csv', { type: 'text/csv' });
    component.selectedFile = file;
    component.destino = 'conta';
    component.config.contaId = 'conta-1';
    
    financeiroService.getPreviewImportacao.and.returnValue(of({ transacoes: [], linhasComErro: 0, totalComSugestao: 0, totalSemSugestao: 0 }));

    component.gerarPreview();

    expect(financeiroService.getPreviewImportacao).toHaveBeenCalledWith(file, 'conta-1', undefined);
  });

  it('deve chamar o servico de confirmacao ao clicar em salvar', () => {
    component.selectedFile = new File(['conteudo'], 'extrato.csv', { type: 'text/csv' });
    component.destino = 'conta';
    component.config.contaId = 'conta-1';
    const mockPreview = [{ idTemporario: '1', descricao: 'T', valor: 10, data: '2024-01-01', categoriasSugeridas: [] }];
    component.previewItems.set(mockPreview);

    financeiroService.confirmarImportacao.and.returnValue(of({ sucesso: true, totalImportado: 1, totalIgnorado: 0 }));
    spyOn(component.imported, 'emit');

    component.confirmarImportacao();

    expect(financeiroService.confirmarImportacao).toHaveBeenCalledWith(mockPreview, 'conta-1', undefined);
    expect(component.imported.emit).toHaveBeenCalled();
  });

  it('deve habilitar o botao de verificar quando houver arquivo e destino selecionados', () => {
    component.selectedFile = new File(['conteudo'], 'extrato.csv', { type: 'text/csv' });
    component.destino = 'conta';
    component.config.contaId = 'conta-1';
    
    fixture.detectChanges();
    expect(component.canProcess()).toBeTrue();
  });
});
