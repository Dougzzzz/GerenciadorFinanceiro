import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TransacoesListComponent } from './transacoes-list.component';
import { FormsModule } from '@angular/forms';
import { TipoTransacao } from '../../core/models/financeiro.model';
import { By } from '@angular/platform-browser';

describe('TransacoesListComponent', () => {
  let component: TransacoesListComponent;
  let fixture: ComponentFixture<TransacoesListComponent>;

  const mockTransacoes = [
    {
      id: '1',
      data: '2026-04-17T00:00:00Z',
      descricao: 'Teste',
      valor: 100,
      tipo: TipoTransacao.Receita,
      categoriaId: 'cat-1'
    } as any
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransacoesListComponent, FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(TransacoesListComponent);
    component = fixture.componentInstance;
    component.transacoes = mockTransacoes;
    component.selecionadas = new Set();
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit onSort with Asc when clicking new column', () => {
    spyOn(component.onSort, 'emit');
    component.ordenarPor = 'Descricao';
    component.direcao = 'Desc';
    
    component.toggleSort('Data');
    
    expect(component.onSort.emit).toHaveBeenCalledWith({ coluna: 'Data', direcao: 'Asc' });
  });

  it('should emit onSort with inverted direction when clicking same column', () => {
    spyOn(component.onSort, 'emit');
    component.ordenarPor = 'Data';
    component.direcao = 'Desc';
    
    component.toggleSort('Data');
    
    expect(component.onSort.emit).toHaveBeenCalledWith({ coluna: 'Data', direcao: 'Asc' });
  });

  it('should return correct sort icon', () => {
    component.ordenarPor = 'Data';
    component.direcao = 'Desc';
    
    expect(component.getSortIcon('Data')).toBe('🔽');
    expect(component.getSortIcon('Descricao')).toBe('↕️');
    
    component.direcao = 'Asc';
    expect(component.getSortIcon('Data')).toBe('🔼');
  });

  it('should render edit inputs when editandoId matches', async () => {
    component.editandoId = '1';
    component.tempEdit = { ...mockTransacoes[0], data: '2026-04-17' };
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const input = fixture.debugElement.query(By.css('input[type="text"]'));
    expect(input).toBeTruthy();
    expect(input.nativeElement.value).toBe('Teste');
  });

  it('should emit onEdit when edit button is clicked', () => {
    spyOn(component.onEdit, 'emit');
    const editBtn = fixture.debugElement.query(By.css('.btn-icon'));
    editBtn.nativeElement.click();
    expect(component.onEdit.emit).toHaveBeenCalledWith(mockTransacoes[0]);
  });
});
