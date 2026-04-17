import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ContasComponent } from './contas.component';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { of } from 'rxjs';

describe('ContasComponent', () => {
  let component: ContasComponent;
  let fixture: ComponentFixture<ContasComponent>;
  let financeiroServiceSpy: jasmine.SpyObj<FinanceiroService>;

  const mockContas = [
    { id: '1', nomeBanco: 'Nubank', saldo: 1000, provedor: 1 }
  ];

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('FinanceiroService', ['getContas', 'criarConta']);

    await TestBed.configureTestingModule({
      imports: [ContasComponent],
      providers: [
        { provide: FinanceiroService, useValue: spy }
      ]
    }).compileComponents();

    financeiroServiceSpy = TestBed.inject(FinanceiroService) as jasmine.SpyObj<FinanceiroService>;
    financeiroServiceSpy.getContas.and.returnValue(of(mockContas as any));
    
    fixture = TestBed.createComponent(ContasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load accounts', () => {
    expect(component).toBeTruthy();
    expect(financeiroServiceSpy.getContas).toHaveBeenCalled();
    expect(component.contas().length).toBe(1);
  });

  it('should call criarConta and reload on salvar', () => {
    const novaConta = { nomeBanco: 'Inter', saldoInicial: 500, provedor: 2 };
    financeiroServiceSpy.criarConta.and.returnValue(of({ id: '2', ...novaConta } as any));
    
    component.salvar(novaConta);
    
    expect(financeiroServiceSpy.criarConta).toHaveBeenCalledWith('Inter', 500, 2);
    expect(financeiroServiceSpy.getContas).toHaveBeenCalledTimes(2);
  });
});
