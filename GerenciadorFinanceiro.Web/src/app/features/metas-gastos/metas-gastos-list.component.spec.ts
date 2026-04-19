import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MetasGastosListComponent } from './metas-gastos-list.component';
import { By } from '@angular/platform-browser';
import { MetaGasto, Categoria, TipoTransacao } from '../../core/models/financeiro.model';

describe('MetasGastosListComponent', () => {
  let component: MetasGastosListComponent;
  let fixture: ComponentFixture<MetasGastosListComponent>;

  const mockCategorias: Categoria[] = [{ id: 'cat-1', nome: 'Alimentação', tipo: TipoTransacao.Despesa }];
  const mockMetas: MetaGasto[] = [
    { id: 'meta-1', categoriaId: 'cat-1', valorLimite: 1000, ehRecorrente: true, mes: undefined, ano: undefined }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MetasGastosListComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(MetasGastosListComponent);
    component = fixture.componentInstance;
    component.metas = mockMetas;
    component.categorias = mockCategorias;
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
    spyOn(component.editClicked, 'emit');
    spyOn(component.deleteClicked, 'emit');

    const editBtn = fixture.debugElement.query(By.css('button[title="Editar"]'));
    const deleteBtn = fixture.debugElement.query(By.css('button[title="Excluir"]'));

    editBtn.nativeElement.click();
    deleteBtn.nativeElement.click();

    expect(component.editClicked.emit).toHaveBeenCalledWith(mockMetas[0]);
    expect(component.deleteClicked.emit).toHaveBeenCalledWith(mockMetas[0]);
  });
});
