import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MetasGastosListComponent } from './metas-gastos-list.component';
import { By } from '@angular/platform-browser';

describe('MetasGastosListComponent', () => {
  let component: MetasGastosListComponent;
  let fixture: ComponentFixture<MetasGastosListComponent>;

  const mockCategorias = [{ id: 'cat-1', nome: 'Alimentação' }];
  const mockMetas = [
    { id: 'meta-1', categoriaId: 'cat-1', valorLimite: 1000, ehRecorrente: true }
  ] as any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MetasGastosListComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(MetasGastosListComponent);
    component = fixture.componentInstance;
    component.metas = mockMetas;
    component.categorias = mockCategorias as any;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should return correct category name', () => {
    expect(component.getCategoriaNome('cat-1')).toBe('Alimentação');
    expect(component.getCategoriaNome('invalid')).toBe('Desconhecida');
  });

  it('should emit events when buttons are clicked', () => {
    spyOn(component.onIniciarEdicao, 'emit');
    spyOn(component.onExcluirUma, 'emit');

    const editBtn = fixture.debugElement.query(By.css('.btn-icon'));
    const deleteBtn = fixture.debugElement.query(By.css('.btn-icon.danger'));

    editBtn.nativeElement.click();
    deleteBtn.nativeElement.click();

    expect(component.onIniciarEdicao.emit).toHaveBeenCalledWith(mockMetas[0]);
    expect(component.onExcluirUma.emit).toHaveBeenCalledWith(mockMetas[0]);
  });
});
