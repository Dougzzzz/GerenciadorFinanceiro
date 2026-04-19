import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CategoriasListComponent } from './categorias-list.component';
import { TipoTransacao } from '../../core/models/financeiro.model';
import { By } from '@angular/platform-browser';
import { signal } from '@angular/core';

describe('CategoriasListComponent', () => {
  let component: CategoriasListComponent;
  let fixture: ComponentFixture<CategoriasListComponent>;

  const mockCategorias = [
    { id: '1', nome: 'Alimentação', tipo: TipoTransacao.Despesa },
    { id: '2', nome: 'Salário', tipo: TipoTransacao.Receita }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoriasListComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(CategoriasListComponent);
    component = fixture.componentInstance;
    component.categorias = mockCategorias;
    component.selecionadas = new Set(['1']);
    component.filtrar();
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit onIniciarEdicao when edit button is clicked', () => {
    spyOn(component.onIniciarEdicao, 'emit');
    // Procurar botão com title "Editar"
    const editBtn = fixture.debugElement.query(By.css('button[title="Editar"]'));
    editBtn.nativeElement.click();
    expect(component.onIniciarEdicao.emit).toHaveBeenCalledWith(mockCategorias[0]);
  });

  it('should emit onExcluirUma when delete button is clicked', () => {
    spyOn(component.onExcluirUma, 'emit');
    // Procurar botão com title "Excluir"
    const deleteBtn = fixture.debugElement.query(By.css('button[title="Excluir"]'));
    deleteBtn.nativeElement.click();
    expect(component.onExcluirUma.emit).toHaveBeenCalledWith(mockCategorias[0]);
  });

  it('should emit onToggleSelecionada when checkbox is changed', () => {
    spyOn(component.onToggleSelecionada, 'emit');
    // Procurar checkbox dentro de .item-info
    const checkbox = fixture.debugElement.query(By.css('.item-info input[type="checkbox"]'));
    checkbox.nativeElement.click();
    expect(component.onToggleSelecionada.emit).toHaveBeenCalledWith('1');
  });

  it('should filter categories when filtro is changed', () => {
    component.filtro = 'Salário';
    component.filtrar();
    fixture.detectChanges();
    
    expect(component.categoriasFiltradas.length).toBe(1);
    expect(component.categoriasFiltradas[0].nome).toBe('Salário');
  });
});
