import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CartoesComponent } from './cartoes.component';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { of } from 'rxjs';

describe('CartoesComponent', () => {
  let component: CartoesComponent;
  let fixture: ComponentFixture<CartoesComponent>;
  let financeiroServiceSpy: jasmine.SpyObj<FinanceiroService>;

  const mockCartoes = [
    { id: '1', nome: 'Visa Platinum', limite: 5000, diaFechamento: 1, diaVencimento: 10, provedor: 1 }
  ];

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('FinanceiroService', ['getCartoes', 'criarCartao']);

    await TestBed.configureTestingModule({
      imports: [CartoesComponent],
      providers: [
        { provide: FinanceiroService, useValue: spy }
      ]
    }).compileComponents();

    financeiroServiceSpy = TestBed.inject(FinanceiroService) as jasmine.SpyObj<FinanceiroService>;
    financeiroServiceSpy.getCartoes.and.returnValue(of(mockCartoes as any));
    
    fixture = TestBed.createComponent(CartoesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load cards', () => {
    expect(component).toBeTruthy();
    expect(financeiroServiceSpy.getCartoes).toHaveBeenCalled();
    expect(component.cartoes().length).toBe(1);
  });

  it('should call criarCartao and reload on salvar', () => {
    const novoCartao = { nome: 'Mastercard Black', limite: 10000, diaFechamento: 5, diaVencimento: 15, provedor: 2 };
    financeiroServiceSpy.criarCartao.and.returnValue(of({ id: '2', ...novoCartao } as any));
    
    component.salvar(novoCartao);
    
    expect(financeiroServiceSpy.criarCartao).toHaveBeenCalled();
    expect(financeiroServiceSpy.getCartoes).toHaveBeenCalledTimes(2);
  });
});
