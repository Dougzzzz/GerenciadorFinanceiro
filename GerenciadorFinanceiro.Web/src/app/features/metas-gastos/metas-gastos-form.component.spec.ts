import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MetasGastosFormComponent } from './metas-gastos-form.component';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';

describe('MetasGastosFormComponent', () => {
  let component: MetasGastosFormComponent;
  let fixture: ComponentFixture<MetasGastosFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MetasGastosFormComponent, FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(MetasGastosFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit onSalvar when form is submitted', () => {
    spyOn(component.onSalvar, 'emit');
    component.novo = { categoriaId: 'cat-1', valorLimite: 500 };
    fixture.detectChanges();

    const form = fixture.debugElement.query(By.css('form'));
    form.triggerEventHandler('ngSubmit', null);

    expect(component.onSalvar.emit).toHaveBeenCalledWith(component.novo);
  });

  it('should disable category select when editing', async () => {
    component.editando = true;
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const select = fixture.debugElement.query(By.css('select')).nativeElement;
    expect(select.disabled).toBeTrue();
  });
});
