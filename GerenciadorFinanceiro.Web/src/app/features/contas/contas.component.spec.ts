import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ContasComponent } from './contas.component';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { of } from 'rxjs';
import { ContaBancaria } from '../../core/models/financeiro.model';

describe('ContasComponent', () => {
  let component: ContasComponent;
  let fixture: ComponentFixture<ContasComponent>;
  let financeiroServiceSpy: jasmine.SpyObj<FinanceiroService>;

  const mockContas: ContaBancaria[] = [
    { id: '1', nomeBanco: 'Nubank', saldoAtual: 1000, provedor: 1 }
  ];

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('FinanceiroService', ['getContas', 'adicionarConta', 'atualizarConta', 'excluirConta']);

    await TestBed.configureTestingModule({
      imports: [ContasComponent],
      providers: [
        { provide: FinanceiroService, useValue: spy }
      ]
    }).compileComponents();

    financeiroServiceSpy = TestBed.inject(FinanceiroService) as jasmine.SpyObj<FinanceiroService>;
    financeiroServiceSpy.getContas.and.returnValue(of(mockContas));
    
    fixture = TestBed.createComponent(ContasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load accounts', () => {
    expect(component).toBeTruthy();
    expect(financeiroServiceSpy.getContas).toHaveBeenCalled();
    expect(component.contas().length).toBe(1);
  });

  it('should call adicionarConta when not editing', () => {
    const novaConta = { nomeBanco: 'Inter', saldoInicial: 500, provedor: 2 };
    financeiroServiceSpy.adicionarConta.and.returnValue(of({ id: '2', ...novaConta, saldoAtual: 500 } as ContaBancaria));
    
    component.salvar(novaConta);
    
    expect(financeiroServiceSpy.adicionarConta).toHaveBeenCalled();
    expect(financeiroServiceSpy.getContas).toHaveBeenCalledTimes(2);
  });

  it('should call atualizarConta when editing', () => {
    const contaEditada = { id: '1', nomeBanco: 'Nubank Atualizado', saldoAtual: 1200, provedor: 1 };
    component.editando.set(contaEditada);
    
    financeiroServiceSpy.atualizarConta.and.returnValue(of({}));
    
    component.salvar({ nomeBanco: 'Nubank Atualizado', saldoInicial: 1200, provedor: 1 });
    
    expect(financeiroServiceSpy.atualizarConta).toHaveBeenCalledWith('1', 'Nubank Atualizado', 1200, 1);
  });

  it('should call excluirConta', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    financeiroServiceSpy.excluirConta.and.returnValue(of({}));
    
    component.excluir('1');
    
    expect(financeiroServiceSpy.excluirConta).toHaveBeenCalledWith('1');
  });
});
