import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MetasGastosComponent } from './metas-gastos.component';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { of, throwError } from 'rxjs';
import { TipoTransacao, MetaGasto, Categoria } from '../../core/models/financeiro.model';

describe('MetasGastosComponent', () => {
  let component: MetasGastosComponent;
  let fixture: ComponentFixture<MetasGastosComponent>;
  let financeiroServiceSpy: jasmine.SpyObj<FinanceiroService>;

  const mockCategorias: Categoria[] = [
    { id: 'cat-1', nome: 'Alimentação', tipo: TipoTransacao.Despesa },
    { id: 'cat-2', nome: 'Salário', tipo: TipoTransacao.Receita }
  ];

  const mockMetas: MetaGasto[] = [
    { id: 'meta-1', categoriaId: 'cat-1', valorLimite: 1000, ehRecorrente: true, mes: undefined, ano: undefined }
  ];

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('FinanceiroService', [
      'getMetas', 
      'getCategorias', 
      'criarMeta', 
      'atualizarMeta', 
      'excluirMeta'
    ]);

    await TestBed.configureTestingModule({
      imports: [MetasGastosComponent],
      providers: [
        { provide: FinanceiroService, useValue: spy }
      ]
    }).compileComponents();

    financeiroServiceSpy = TestBed.inject(FinanceiroService) as jasmine.SpyObj<FinanceiroService>;
    financeiroServiceSpy.getMetas.and.returnValue(of(mockMetas));
    financeiroServiceSpy.getCategorias.and.returnValue(of(mockCategorias));
    
    fixture = TestBed.createComponent(MetasGastosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load data', () => {
    expect(component).toBeTruthy();
    expect(financeiroServiceSpy.getMetas).toHaveBeenCalled();
    expect(financeiroServiceSpy.getCategorias).toHaveBeenCalled();
    expect(component.metas().length).toBe(1);
    expect(component.categoriasDespesa.length).toBe(1);
  });

  it('should start editing a meta', () => {
    const meta = mockMetas[0];
    component.iniciarEdicao(meta);
    
    expect(component.editando()).toEqual(meta);
    expect(component.novo.categoriaId).toBe(meta.categoriaId);
  });

  it('should clear form', () => {
    component.iniciarEdicao(mockMetas[0]);
    component.limpar();
    
    expect(component.editando()).toBeNull();
    expect(component.novo.categoriaId).toBe('');
  });

  it('should toggle selection', () => {
    component.toggleSelecionada('meta-1');
    expect(component.selecionadas().has('meta-1')).toBeTrue();
    
    component.toggleSelecionada('meta-1');
    expect(component.selecionadas().has('meta-1')).toBeFalse();
  });

  it('should call criarMeta on onSave when not editing', () => {
    const novaMeta = { categoriaId: 'cat-1', valorLimite: 200 };
    financeiroServiceSpy.criarMeta.and.returnValue(of({ id: 'meta-2', ...novaMeta } as MetaGasto));
    
    component.onSave(novaMeta);
    
    expect(financeiroServiceSpy.criarMeta).toHaveBeenCalledWith(novaMeta);
  });

  it('should call atualizarMeta on onSave when editing', () => {
    const meta = mockMetas[0];
    component.iniciarEdicao(meta);
    const dadosAlt = { valorLimite: 1500 };
    financeiroServiceSpy.atualizarMeta.and.returnValue(of({ ...meta, valorLimite: 1500 } as MetaGasto));
    
    component.onSave(dadosAlt);
    
    expect(financeiroServiceSpy.atualizarMeta).toHaveBeenCalledWith(meta.id, 1500);
  });

  it('should delete meta and reload data', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    financeiroServiceSpy.excluirMeta.and.returnValue(of({}));
    
    component.onDelete(mockMetas[0]);
    
    expect(financeiroServiceSpy.excluirMeta).toHaveBeenCalledWith('meta-1');
    expect(financeiroServiceSpy.getMetas).toHaveBeenCalledTimes(2);
  });

  it('should handle error on save', () => {
    spyOn(window, 'alert');
    financeiroServiceSpy.criarMeta.and.returnValue(throwError(() => new Error('Erro de API')));
    
    component.onSave({ categoriaId: 'cat-1', valorLimite: 100 });
    
    expect(window.alert).toHaveBeenCalledWith('Erro ao salvar meta: Erro de API');
  });
});
