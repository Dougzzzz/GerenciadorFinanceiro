import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TransacoesListComponent } from './transacoes-list.component';
import { FormsModule } from '@angular/forms';
import { TipoTransacao, Transacao } from '../../core/models/financeiro.model';
import { By } from '@angular/platform-browser';

describe('TransacoesListComponent', () => {
  let component: TransacoesListComponent;
  let fixture: ComponentFixture<TransacoesListComponent>;

  const mockTransacoes: Transacao[] = [
    {
      id: '1',
      data: '2026-04-17T00:00:00Z',
      descricao: 'Teste',
      valor: 100,
      tipo: TipoTransacao.Receita,
      categoriaId: 'cat-1',
      categoria: 'Outros',
      nomeCartao: '',
      finalCartao: '',
      parcela: '',
      cotacao: 1
    }
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

  it('should emit sort with Asc when clicking new column', () => {
    spyOn(component.sort, 'emit');
    component.ordenarPor = 'Descricao';
    component.direcao = 'Desc';
    
    component.toggleSort('Data');
    
    expect(component.sort.emit).toHaveBeenCalledWith({ coluna: 'Data', direcao: 'Asc' });
  });

  it('should emit sort with inverted direction when clicking same column', () => {
    spyOn(component.sort, 'emit');
    component.ordenarPor = 'Data';
    component.direcao = 'Desc';
    
    component.toggleSort('Data');
    
    expect(component.sort.emit).toHaveBeenCalledWith({ coluna: 'Data', direcao: 'Asc' });
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

    const input = fixture.debugElement.query(By.css('input[aria-label="Editar descrição"]'));
    expect(input).toBeTruthy();
    expect(input.nativeElement.value).toBe('Teste');
  });

  it('should emit edit when edit button is clicked', () => {
    spyOn(component.edit, 'emit');
    const editBtn = fixture.debugElement.query(By.css('.btn-icon'));
    editBtn.nativeElement.click();
    expect(component.edit.emit).toHaveBeenCalledWith(mockTransacoes[0]);
  });
});
