import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CategoriasComponent } from './categorias.component';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { of, throwError } from 'rxjs';
import { TipoTransacao } from '../../core/models/financeiro.model';

describe('CategoriasComponent', () => {
  let component: CategoriasComponent;
  let fixture: ComponentFixture<CategoriasComponent>;
  let financeiroServiceSpy: jasmine.SpyObj<FinanceiroService>;

  const mockCategorias = [
    { id: '1', nome: 'Alimentação', tipo: TipoTransacao.Despesa },
    { id: '2', nome: 'Salário', tipo: TipoTransacao.Receita }
  ];

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('FinanceiroService', [
      'getCategorias', 
      'criarCategoria', 
      'atualizarCategoria', 
      'excluirCategorias'
    ]);

    await TestBed.configureTestingModule({
      imports: [CategoriasComponent],
      providers: [
        { provide: FinanceiroService, useValue: spy }
      ]
    }).compileComponents();

    financeiroServiceSpy = TestBed.inject(FinanceiroService) as jasmine.SpyObj<FinanceiroService>;
    financeiroServiceSpy.getCategorias.and.returnValue(of(mockCategorias));
    
    fixture = TestBed.createComponent(CategoriasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load categories', () => {
    expect(component).toBeTruthy();
    expect(financeiroServiceSpy.getCategorias).toHaveBeenCalled();
    expect(component.categorias().length).toBe(2);
  });

  it('should start editing a category', () => {
    const cat = mockCategorias[0];
    component.iniciarEdicao(cat);
    
    expect(component.editando()).toEqual(cat);
    expect(component.novo.nome).toBe(cat.nome);
  });

  it('should clear form', () => {
    component.iniciarEdicao(mockCategorias[0]);
    component.limpar();
    
    expect(component.editando()).toBeNull();
    expect(component.novo.nome).toBe('');
  });

  it('should toggle selection of a category', () => {
    component.toggleSelecionada('1');
    expect(component.selecionadas().has('1')).toBeTrue();
    
    component.toggleSelecionada('1');
    expect(component.selecionadas().has('1')).toBeFalse();
  });

  it('should toggle all categories', () => {
    component.toggleTodas({ target: { checked: true } });
    expect(component.selecionadas().size).toBe(2);

    component.toggleTodas({ target: { checked: false } });
    expect(component.selecionadas().size).toBe(0);
  });

  it('should call excluirCategorias for multiple selected', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    spyOn(window, 'alert');
    component.selecionadas.set(new Set(['1', '2']));
    financeiroServiceSpy.excluirCategorias.and.returnValue(of({}));
    
    component.excluirSelecionadas();
    
    expect(financeiroServiceSpy.excluirCategorias).toHaveBeenCalledWith(['1', '2']);
  });

  it('should call criarCategoria on salvar when not editing', () => {
    component.novo = { nome: 'Nova', tipo: TipoTransacao.Despesa };
    financeiroServiceSpy.criarCategoria.and.returnValue(of({ id: '3', ...component.novo }));
    
    component.salvar();
    
    expect(financeiroServiceSpy.criarCategoria).toHaveBeenCalledWith('Nova', TipoTransacao.Despesa);
  });

  it('should call atualizarCategoria on salvar when editing', () => {
    const cat = mockCategorias[0];
    component.iniciarEdicao(cat);
    component.novo.nome = 'Alterado';
    financeiroServiceSpy.atualizarCategoria.and.returnValue(of({ ...cat, nome: 'Alterado' }));
    
    component.salvar();
    
    expect(financeiroServiceSpy.atualizarCategoria).toHaveBeenCalled();
  });

  it('should handle error on delete', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    spyOn(window, 'alert');
    financeiroServiceSpy.excluirCategorias.and.returnValue(throwError(() => ({ error: 'Erro de teste' })));
    
    component.excluirUma(mockCategorias[0]);
    
    expect(window.alert).toHaveBeenCalledWith('Erro de teste');
  });
});
