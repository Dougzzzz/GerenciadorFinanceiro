import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CartoesFormComponent } from './cartoes-form.component';
import { FormsModule } from '@angular/forms';

describe('CartoesFormComponent', () => {
  let component: CartoesFormComponent;
  let fixture: ComponentFixture<CartoesFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartoesFormComponent, FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(CartoesFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit onSalvar and reset form when salvar is called', () => {
    spyOn(component.onSalvar, 'emit');
    component.novo = { nome: 'Visa', limite: 1000, diaFechamento: 1, diaVencimento: 10, provedor: 1 };
    
    component.salvar();
    
    expect(component.onSalvar.emit).toHaveBeenCalledWith({ nome: 'Visa', limite: 1000, diaFechamento: 1, diaVencimento: 10, provedor: 1 });
    expect(component.novo.nome).toBe('');
  });
});
