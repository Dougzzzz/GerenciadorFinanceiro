import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CategoriasFormComponent } from './categorias-form.component';
import { FormsModule } from '@angular/forms';
import { TipoTransacao } from '../../core/models/financeiro.model';
import { By } from '@angular/platform-browser';

describe('CategoriasFormComponent', () => {
  let component: CategoriasFormComponent;
  let fixture: ComponentFixture<CategoriasFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoriasFormComponent, FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(CategoriasFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit saved when form is submitted', () => {
    spyOn(component.saved, 'emit');
    component.novo = { nome: 'Teste', tipo: TipoTransacao.Despesa };
    fixture.detectChanges();

    const form = fixture.nativeElement.querySelector('form');
    form.dispatchEvent(new Event('submit'));

    expect(component.saved.emit).toHaveBeenCalledWith(component.novo);
  });

  it('should emit cleared when cancel button is clicked', () => {
    spyOn(component.cleared, 'emit');
    component.editando = true;
    fixture.detectChanges();

    const cancelBtn = fixture.debugElement.query(By.css('app-button[variant="outline"]'));
    cancelBtn.triggerEventHandler('clicked', null);

    expect(component.cleared.emit).toHaveBeenCalled();
  });

  it('should disable submit button when name is empty', () => {
    component.novo = { nome: '', tipo: TipoTransacao.Despesa };
    fixture.detectChanges();

    const submitBtn = fixture.nativeElement.querySelector('app-button[type="submit"]');
    expect(submitBtn.getAttribute('ng-reflect-disabled')).toBe('true');
  });
});
