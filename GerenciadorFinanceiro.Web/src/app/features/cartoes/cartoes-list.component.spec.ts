import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CartoesListComponent } from './cartoes-list.component';
import { ProvedorExtrato } from '../../core/models/financeiro.model';

describe('CartoesListComponent', () => {
  let component: CartoesListComponent;
  let fixture: ComponentFixture<CartoesListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartoesListComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(CartoesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render cards list', () => {
    component.cartoes = [{ id: '1', nome: 'Mastercard', limite: 5000, diaFechamento: 5, diaVencimento: 15, provedor: ProvedorExtrato.Nubank }];
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.item')).toBeTruthy();
    expect(compiled.querySelector('strong')?.textContent).toContain('Mastercard');
  });
});
