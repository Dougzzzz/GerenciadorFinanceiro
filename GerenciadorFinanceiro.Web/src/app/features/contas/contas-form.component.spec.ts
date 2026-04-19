import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ContasFormComponent } from './contas-form.component';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';

describe('ContasFormComponent', () => {
  let component: ContasFormComponent;
  let fixture: ComponentFixture<ContasFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContasFormComponent, FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(ContasFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit saved when salvar is called', () => {
    spyOn(component.saved, 'emit');
    component.novo = { nomeBanco: 'Inter', saldoInicial: 100, provedor: 3 };
    
    component.salvar();
    
    expect(component.saved.emit).toHaveBeenCalledWith({ nomeBanco: 'Inter', saldoInicial: 100, provedor: 3 });
  });

  it('should emit cleared when cancel button is clicked', () => {
    spyOn(component.cleared, 'emit');
    component.editando = true;
    fixture.detectChanges();
    
    const cancelBtn = fixture.debugElement.queryAll(By.css('app-button'))[1]; // Segundo botão (Cancelar)
    cancelBtn.triggerEventHandler('clicked', null);
    
    expect(component.cleared.emit).toHaveBeenCalled();
  });
});
