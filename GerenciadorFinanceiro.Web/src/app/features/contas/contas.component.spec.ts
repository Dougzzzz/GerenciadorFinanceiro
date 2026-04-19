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
    const spy = jasmine.createSpyObj('FinanceiroService', ['getContas', 'criarConta', 'atualizarConta', 'excluirConta']);

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

  it('should call criarConta when not editing', () => {
    const novaConta = { nomeBanco: 'Inter', saldoInicial: 500, provedor: 2 };
    financeiroServiceSpy.criarConta.and.returnValue(of({ id: '2', ...novaConta, saldoAtual: 500 } as ContaBancaria));
    
    component.salvar(novaConta);
    
    expect(financeiroServiceSpy.criarConta).toHaveBeenCalled();
    expect(financeiroServiceSpy.getContas).toHaveBeenCalledTimes(2);
  });

  it('should call atualizarConta when editing', () => {
    const conta = mockContas[0];
    component.iniciarEdicao(conta);
    
    const dadosAlt = { nomeBanco: 'Nubank Alt', saldoInicial: 2000, provedor: 1 };
    financeiroServiceSpy.atualizarConta.and.returnValue(of({}));
    
    component.salvar(dadosAlt);
    
    expect(financeiroServiceSpy.atualizarConta).toHaveBeenCalledWith('1', 'Nubank Alt', 2000, 1);
  });

  it('should call excluirConta after confirmation', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    financeiroServiceSpy.excluirConta.and.returnValue(of({}));
    
    component.excluir('1');
    
    expect(financeiroServiceSpy.excluirConta).toHaveBeenCalledWith('1');
    expect(financeiroServiceSpy.getContas).toHaveBeenCalledTimes(2);
  });

  it('should clear editing state when limpar is called', () => {
    component.iniciarEdicao(mockContas[0]);
    component.limpar();
    
    expect(component.editando()).toBeNull();
    expect(component.novo().nomeBanco).toBe('');
  });
});
